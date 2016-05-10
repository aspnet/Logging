// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventSourceLogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;


namespace Microsoft.Extensions.Logging.Test
{
    public class EventSourceLoggerTest
    {
        [Fact]
        public void Logs_AsExpected_WithDefaults()
        {
            TestEventListener.Settings.Keywords = EventKeywords.None;
            TestEventListener.Settings.FilterSpec = null;
            TestEventListener.Settings.Level = default(EventLevel);
            using (var testListener = new TestEventListener())
            {
                var factory = new LoggerFactory();
                factory.AddEventSourceLogger();

                LogStuff(factory);

                // Use testListener.DumpEvents as necessary to examine what exactly the listener received

#if NO_EVENTSOURCE_COMPLEX_TYPE_SUPPORT
                VerifyEvents(testListener, 
                    "E1FM", "E1JS",         
                     // Second event is omitted because default LogLevel == Debug 
                    "E3FM", "E3JS",         
                    "OuterScopeJsonStart",  
                    "E4FM", "E4JS",         
                    "E5FM", "E5JS",         
                    "InnerScopeJsonStart",
                    "E6FM", "E6JS", 
                    "InnerScopeJsonStop", 
                    "E7FM", "E7JS",
                    "OuterScopeJsonStop", 
                    "E8FM", "E8JS");
#else
                VerifyEvents(testListener,
                    "E1FM", "E1MSG", "E1JS",
                    // Second event is omitted because default LogLevel == Debug 
                    "E3FM", "E3MSG", "E3JS",
                    "OuterScopeJsonStart",
                    "E4FM", "E4MSG", "E4JS",
                    "E5FM", "E5MSG", "E5JS",
                    "InnerScopeJsonStart",
                    "E6FM", "E6MSG", "E6JS",
                    "InnerScopeJsonStop",
                    "E7FM", "E7MSG", "E7JS",
                    "OuterScopeJsonStop",
                    "E8FM", "E8MSG", "E8JS");
#endif
            }
        }

        [Fact]
        public void Logs_Nothing_IfNotEnabled()
        {
            TestEventListener.Settings.Keywords = EventKeywords.None;
            TestEventListener.Settings.FilterSpec = null;
            TestEventListener.Settings.Level = default(EventLevel);
            using (var testListener = new TestEventListener())
            {
                var factory = new LoggerFactory();
                // No call to factory.AddEventSourceLogger();

                LogStuff(factory);

                VerifyEvents(testListener); // No verifiers = 0 events expected
            }
        }

        [Fact]
        public void Logs_OnlyFormattedMessage_IfKeywordSet()
        {
            TestEventListener.Settings.Keywords = LoggingEventSource.Keywords.FormattedMessage;
            TestEventListener.Settings.FilterSpec = null;
            TestEventListener.Settings.Level = EventLevel.Verbose;
            using (var testListener = new TestEventListener())
            {
                var factory = new LoggerFactory();
                factory.AddEventSourceLogger();

                LogStuff(factory);

                VerifyEvents(testListener,
                    "E1FM",
                    // Second event is omitted because default LogLevel == Debug 
                    "E3FM", 
                    "OuterScopeStart",
                    "E4FM", 
                    "E5FM", 
                    "InnerScopeStart",
                    "E6FM", 
                    "InnerScopeStop",
                    "E7FM", 
                    "OuterScopeStop",
                    "E8FM");
            }
        }

        [Fact]
        public void Logs_OnlyJson_IfKeywordSet()
        {
            TestEventListener.Settings.Keywords = LoggingEventSource.Keywords.JsonMessage;
            TestEventListener.Settings.FilterSpec = null;
            TestEventListener.Settings.Level = EventLevel.Verbose;
            using (var testListener = new TestEventListener())
            {
                var factory = new LoggerFactory();
                factory.AddEventSourceLogger();

                LogStuff(factory);

                VerifyEvents(testListener,
                    "E1JS",
                    // Second event is omitted because default LogLevel == Debug 
                    "E3JS",
                    "OuterScopeJsonStart",
                    "E4JS",
                    "E5JS",
                    "InnerScopeJsonStart",
                    "E6JS",
                    "InnerScopeJsonStop",
                    "E7JS",
                    "OuterScopeJsonStop",
                    "E8JS");
            }
        }

        [Fact]
        public void Logs_OnlyMessage_IfKeywordSet()
        {
            TestEventListener.Settings.Keywords = LoggingEventSource.Keywords.Message;
            TestEventListener.Settings.FilterSpec = null;
            TestEventListener.Settings.Level = EventLevel.Verbose;
            using (var testListener = new TestEventListener())
            {
                var factory = new LoggerFactory();
                factory.AddEventSourceLogger();

                LogStuff(factory);

                VerifyEvents(testListener,
#if !NO_EVENTSOURCE_COMPLEX_TYPE_SUPPORT
                    "E1MSG",
                    // Second event is omitted because default LogLevel == Debug 
                    "E3MSG",
#endif
                    "OuterScopeStart",
#if !NO_EVENTSOURCE_COMPLEX_TYPE_SUPPORT
                    "E4MSG",
                    "E5MSG",
#endif
                    "InnerScopeStart",
#if !NO_EVENTSOURCE_COMPLEX_TYPE_SUPPORT
                    "E6MSG",
#endif
                    "InnerScopeStop",
#if !NO_EVENTSOURCE_COMPLEX_TYPE_SUPPORT
                    "E7MSG",
#endif
                    "OuterScopeStop"
#if !NO_EVENTSOURCE_COMPLEX_TYPE_SUPPORT
                    ,"E8MSG"
#endif
                    );
            }
        }

        [Fact]
        public void Logs_AllEvents_IfTraceSet()
        {
            TestEventListener.Settings.Keywords = LoggingEventSource.Keywords.JsonMessage;
            TestEventListener.Settings.FilterSpec = "Logger1:Trace;Logger2:Trace;Logger3:Trace";
            TestEventListener.Settings.Level = EventLevel.Verbose;
            using (var testListener = new TestEventListener())
            {
                var factory = new LoggerFactory();
                factory.AddEventSourceLogger();

                LogStuff(factory);

                VerifyEvents(testListener,
                    "E1JS",
                    "E2JS",
                    "E3JS",
                    "OuterScopeJsonStart",
                    "E4JS",
                    "E5JS",
                    "InnerScopeJsonStart",
                    "E6JS",
                    "InnerScopeJsonStop",
                    "E7JS",
                    "OuterScopeJsonStop",
                    "E8JS");
            }
        }

        [Fact]
        public void Logs_AsExpected_AtErrorLevel()
        {
            TestEventListener.Settings.Keywords = LoggingEventSource.Keywords.JsonMessage;
            TestEventListener.Settings.FilterSpec = null;
            TestEventListener.Settings.Level = EventLevel.Error;
            using (var testListener = new TestEventListener())
            {
                var factory = new LoggerFactory();
                factory.AddEventSourceLogger();

                LogStuff(factory);

                VerifyEvents(testListener,
                    "OuterScopeJsonStart",
                    "E4JS",
                    "E5JS",
                    "InnerScopeJsonStart",
                    "InnerScopeJsonStop",
                    "OuterScopeJsonStop");
            }
        }

        [Fact]
        public void Logs_AsExpected_AtWarningLevel()
        {
            TestEventListener.Settings.Keywords = LoggingEventSource.Keywords.JsonMessage;
            TestEventListener.Settings.FilterSpec = null;
            TestEventListener.Settings.Level = EventLevel.Warning;
            using (var testListener = new TestEventListener())
            {
                var factory = new LoggerFactory();
                factory.AddEventSourceLogger();

                LogStuff(factory);

                VerifyEvents(testListener,
                    "OuterScopeJsonStart",
                    "E4JS",
                    "E5JS",
                    "InnerScopeJsonStart",
                    "E6JS",
                    "InnerScopeJsonStop",
                    "OuterScopeJsonStop",
                    "E8JS");
            }
        }

        [Fact]
        public void Logs_AsExpected_WithSingleLoggerSpec()
        {
            TestEventListener.Settings.Keywords = LoggingEventSource.Keywords.JsonMessage;
            TestEventListener.Settings.FilterSpec = "Logger2";
            TestEventListener.Settings.Level = EventLevel.Verbose;
            using (var testListener = new TestEventListener())
            {
                var factory = new LoggerFactory();
                factory.AddEventSourceLogger();

                LogStuff(factory);

                VerifyEvents(testListener,
                    "E5JS",
                    "E6JS",
                    "E8JS");
            }
        }

        [Fact]
        public void Logs_AsExpected_WithSingleLoggerSpecWithVerbosity()
        {
            TestEventListener.Settings.Keywords = LoggingEventSource.Keywords.JsonMessage;
            TestEventListener.Settings.FilterSpec = "Logger2:Error";
            TestEventListener.Settings.Level = EventLevel.Error;
            using (var testListener = new TestEventListener())
            {
                var factory = new LoggerFactory();
                factory.AddEventSourceLogger();

                LogStuff(factory);

                VerifyEvents(testListener,
                    "E5JS");
            }
        }

        [Fact]
        public void Logs_AsExpected_WithComplexLoggerSpec()
        {
            TestEventListener.Settings.Keywords = LoggingEventSource.Keywords.JsonMessage;
            TestEventListener.Settings.FilterSpec = "Logger1:Warning;Logger2:Error";
            TestEventListener.Settings.Level = EventLevel.Verbose;
            using (var testListener = new TestEventListener())
            {
                var factory = new LoggerFactory();
                factory.AddEventSourceLogger();

                LogStuff(factory);

                VerifyEvents(testListener,
                    "OuterScopeJsonStart",
                    "E4JS",
                    "E5JS",
                    "OuterScopeJsonStop");
            }
        }


        private void LogStuff(ILoggerFactory factory)
        {
            var logger1 = factory.CreateLogger("Logger1");
            var logger2 = factory.CreateLogger("Logger2");
            var logger3 = factory.CreateLogger("Logger3");

            logger1.LogDebug(new EventId(1), "Logger1 Event1 Debug {intParam}", 1);
            logger2.LogTrace(new EventId(2), "Logger2 Event2 Trace {doubleParam} {timeParam} {doubleParam2}", DoubleParam1, TimeParam.ToString("O"), DoubleParam2);
            logger3.LogInformation(new EventId(3), "Logger3 Event3 Information {string1Param} {string2Param} {string3Param}", "foo", "bar", "baz");

            using (logger1.BeginScope("Outer scope {stringParam} {intParam} {doubleParam}", "scoped foo", 13, DoubleParam1))
            {
                logger1.LogError(new EventId(4), "Logger1 Event4 Error {stringParam} {guidParam}", "foo", GuidParam);

                logger2.LogCritical(new EventId(5), new Exception("oops", new Exception("inner oops")),
                    "Logger2 Event5 Critical {stringParam} {int1Param} {int2Param}", "bar", 23, 45);

                using (logger3.BeginScope("Inner scope {timeParam} {guidParam}", TimeParam, GuidParam))
                {
                    logger2.LogWarning(new EventId(6), "Logger2 Event6 Warning NoParams");
                }

                logger3.LogInformation(new EventId(7), "Logger3 Event7 Information {stringParam} {doubleParam} {intParam}", "inner scope closed", DoubleParam2, 37);
            }

            logger2.LogWarning(new EventId(8), "Logger2 Event8 Warning {stringParam} {timeParam}", "Outer scope closed", TimeParam.ToString("O"));
        }

        private static void VerifyEvents(TestEventListener eventListener, params string[] verifierIDs)
        {
            Assert.Collection(eventListener.Events, verifierIDs.Select(id => EventVerifiers[id]).ToArray());
        }

        private static void VerifySingleEvent(string eventJson, string loggerName, string eventName, int? eventId, LogLevel? level, params string[] fragments)
        {
            Assert.True(eventJson.Contains(@"""__EVENT_NAME"":""" + eventName + @""""), $"Event name does not match. Expected {eventName}, event data is '{eventJson}'");
            Assert.True(eventJson.Contains(@"""LoggerName"":""" + loggerName + @""""), $"Logger name does not match. Expected {loggerName}, event data is '{eventJson}'");
            if (level.HasValue)
            {
                Assert.True(eventJson.Contains(@"""Level"":" + ((int)level.Value).ToString()), $"Log level does not match. Expected level {((int)level.Value).ToString()}, event data is '{eventJson}'");
            }
            if (eventId.HasValue)
            {
                Assert.True(eventJson.Contains(@"""EventId"":""" + eventId.Value.ToString()), $"Event id does not match. Expected id {eventId.Value}, event data is '{eventJson}'");
            }

            for (int i=0; i<fragments.Length; i++)
            {
                Assert.True(eventJson.Contains(fragments[i]), $"Event data '{eventJson}' does not contain expected fragment {fragments[i]}");
            }
        }

        private class TestEventListener : EventListener
        {
            // This is a workaround for a bug in EventListener where OnEventSourceCreated will be called before the listener
            // is fully constructed. Unfortunately this bug has shipped and is now an "expected behavior".
            public class ListenerSettings
            {
                public EventKeywords Keywords;
                public EventLevel Level;
                public string FilterSpec;
            }
            public static ListenerSettings Settings = new ListenerSettings();

            private EventSource _loggingEventSource;

            public TestEventListener()
            {
                Events = new List<string>();
            }

            public List<string> Events;

            public void DumpEvents()
            {
                foreach(string eventData in Events)
                {
                    Console.WriteLine(eventData);
                }
            }

            protected override void OnEventSourceCreated(EventSource eventSource)
            {
                if (eventSource.Name == "Microsoft-Extensions-Logging")
                {
                    _loggingEventSource = eventSource;
                    var args = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(Settings.FilterSpec))
                    {
                        args["FilterSpecs"] = Settings.FilterSpec;
                    }

                    EnableEvents(eventSource, Settings.Level, Settings.Keywords, args);
                }
            }

            public override void Dispose()
            {
                DisableEvents(_loggingEventSource);
                base.Dispose();
            }

            protected override void OnEventWritten(EventWrittenEventArgs eventWrittenArgs)
            {
                // We cannot hold onto EventWrittenEventArgs for long because they are agressively reused.
                StringWriter sw = new StringWriter();
                JsonTextWriter writer = new JsonTextWriter(sw);
                writer.DateFormatString = "O";

                writer.WriteStartObject();
                writer.WritePropertyName("__EVENT_NAME");
                writer.WriteValue(eventWrittenArgs.EventName);

                for (int i = 0; i < eventWrittenArgs.PayloadNames.Count; i++)
                {
                    string propertyName = eventWrittenArgs.PayloadNames[i];
                    writer.WritePropertyName(propertyName, true);
                    if (propertyName.EndsWith("Json"))
                    {
                        writer.WriteRawValue(eventWrittenArgs.Payload[i].ToString());
                    }
                    else
                    {
#if !NO_EVENTSOURCE_COMPLEX_TYPE_SUPPORT
                        if (eventWrittenArgs.Payload[i] == null || eventWrittenArgs.Payload[i].GetType().IsPrimitive)
                        {
                            writer.WriteValue(eventWrittenArgs.Payload[i]);
                        }
                        else if (eventWrittenArgs.Payload[i] is IDictionary<string, object>)
                        {
                            var dictProperty = (IDictionary<string, object>)eventWrittenArgs.Payload[i];
                            // EventPayload claims to support IDictionary<string, object>, but you cannot get a KeyValuePair enumerator out of it
                            // So we need to serialize manually
                            writer.WriteStartObject();

                            for (int j = 0; j < dictProperty.Keys.Count; j++)
                            {
                                writer.WritePropertyName(dictProperty.Keys.ElementAt(j));
                                writer.WriteValue(dictProperty.Values.ElementAt(j));
                            }

                            writer.WriteEndObject();
                        }
                        else
                        {
                            string serializedComplexValue = JsonConvert.SerializeObject(eventWrittenArgs.Payload[i]);
                            writer.WriteRawValue(serializedComplexValue);
                        }
#else
                        writer.WriteValue(eventWrittenArgs.Payload[i]);
#endif
                    }
                }

                writer.WriteEndObject();
                Events.Add(sw.ToString());
            }
        }

        private static class EventTypes
        {
            public static readonly string FormattedMessage = "FormattedMessage";
            public static readonly string MessageJson = "MessageJson";
            public static readonly string Message = "Message";
            public static readonly string ActivityJsonStart = "ActivityJsonStart";
            public static readonly string ActivityJsonStop = "ActivityJsonStop";
            public static readonly string ActivityStart = "ActivityStart";
            public static readonly string ActivityStop = "ActivityStop";
        }

        private static readonly Guid GuidParam = new Guid("29bebd2c-7fa6-4e97-af68-b91fdaae24b6");
        private static readonly double DoubleParam1 = 3.1416;
        private static readonly double DoubleParam2 = -273.15;
        private static readonly DateTime TimeParam = new DateTime(2016, 5, 3, 19, 0, 0, DateTimeKind.Utc);

        private static readonly IDictionary<string, Action<string>> EventVerifiers = new Dictionary<string, Action<string>>
        {
            { "E1FM", (e) => VerifySingleEvent(e, "Logger1", EventTypes.FormattedMessage, 1, LogLevel.Debug,
                @"""FormattedMessage"":""Logger1 Event1 Debug 1""") },
            { "E1JS", (e) => VerifySingleEvent(e, "Logger1", EventTypes.MessageJson, 1, LogLevel.Debug,
                        @"""ArgumentsJson"":{""intParam"":""1""") },
            { "E1MSG", (e) => VerifySingleEvent(e, "Logger1", EventTypes.Message, 1, LogLevel.Debug,
                        @"{""Key"":""intParam"",""Value"":""1""}") },

            { "E2FM", (e) => VerifySingleEvent(e, "Logger2", EventTypes.FormattedMessage, 2, LogLevel.Trace,
                @"""FormattedMessage"":""Logger2 Event2 Trace " + DoubleParam1.ToString() + " " + TimeParam.ToString("O") + " " + DoubleParam2.ToString()) },
            { "E2JS", (e) => VerifySingleEvent(e, "Logger2", EventTypes.MessageJson, 2, LogLevel.Trace,
                        @"""ArgumentsJson"":{""doubleParam"":""" + DoubleParam1.ToString() + @""",""timeParam"":"""
                        + TimeParam.ToString("O") +@""",""doubleParam2"":""" + DoubleParam2.ToString()) },
            { "E2MSG", (e) => VerifySingleEvent(e, "Logger2", EventTypes.Message, 2, LogLevel.Trace,
                @"{""Key"":""doubleParam"",""Value"":""" + DoubleParam1.ToString() +@"""}",
                @"{""Key"":""timeParam"",""Value"":""" + TimeParam.ToString("O") +@"""}",
                @"{""Key"":""doubleParam2"",""Value"":""" + DoubleParam2.ToString() +@"""}") },

            { "E3FM", (e) => VerifySingleEvent(e, "Logger3", EventTypes.FormattedMessage, 3, LogLevel.Information,
                @"""FormattedMessage"":""Logger3 Event3 Information foo bar baz") },
            { "E3JS", (e) => VerifySingleEvent(e, "Logger3", EventTypes.MessageJson, 3, LogLevel.Information,
                        @"""ArgumentsJson"":{""string1Param"":""foo"",""string2Param"":""bar"",""string3Param"":""baz""") },
            { "E3MSG", (e) => VerifySingleEvent(e, "Logger3", EventTypes.Message, 3, LogLevel.Information,
                @"{""Key"":""string1Param"",""Value"":""foo""}", 
                @"{""Key"":""string2Param"",""Value"":""bar""}", 
                @"{""Key"":""string3Param"",""Value"":""baz""}") },

            { "E4FM", (e) => VerifySingleEvent(e, "Logger1", EventTypes.FormattedMessage, 4, LogLevel.Error,
                @"""FormattedMessage"":""Logger1 Event4 Error foo " + GuidParam.ToString("D") + @"""") },
            { "E4JS", (e) => VerifySingleEvent(e, "Logger1", EventTypes.MessageJson, 4, LogLevel.Error,
                @"""ArgumentsJson"":{""stringParam"":""foo"",""guidParam"":""" + GuidParam.ToString("D") + @"""") },
            { "E4MSG", (e) => VerifySingleEvent(e, "Logger1", EventTypes.Message, 4, LogLevel.Error,
                @"{""Key"":""stringParam"",""Value"":""foo""}",
                @"{""Key"":""guidParam"",""Value"":""" + GuidParam.ToString("D") +@"""}") },

            { "E5FM", (e) => VerifySingleEvent(e, "Logger2", EventTypes.FormattedMessage, 5, LogLevel.Critical,
                @"""FormattedMessage"":""Logger2 Event5 Critical bar 23 45") },
            { "E5JS", (e) => VerifySingleEvent(e, "Logger2", EventTypes.MessageJson, 5, LogLevel.Critical,
                @"""ArgumentsJson"":{""stringParam"":""bar"",""int1Param"":""23"",""int2Param"":""45""",
                @"""ExceptionJson"":{""TypeName"":""System.Exception"",""Message"":""oops"",""HResult"":""-2146233088"",""VerboseMessage"":""System.Exception: oops ---> System.Exception: inner oops") },
            { "E5MSG", (e) => VerifySingleEvent(e, "Logger2", EventTypes.Message, 5, LogLevel.Critical,
                 @"{""Key"":""stringParam"",""Value"":""bar""}",
                @"{""Key"":""int1Param"",""Value"":""23""}",
                @"{""Key"":""int2Param"",""Value"":""45""}",
                @"""Exception"":{""TypeName"":""System.Exception"",""Message"":""oops"",""HResult"":-2146233088,""VerboseMessage"":""System.Exception: oops ---> System.Exception: inner oops") },

            { "E6FM", (e) => VerifySingleEvent(e, "Logger2", EventTypes.FormattedMessage, 6, LogLevel.Warning,
                @"""FormattedMessage"":""Logger2 Event6 Warning NoParams""") },
            { "E6JS", (e) => VerifySingleEvent(e, "Logger2", EventTypes.MessageJson, 6, LogLevel.Warning) },
            { "E6MSG", (e) => VerifySingleEvent(e, "Logger2", EventTypes.Message, 6, LogLevel.Warning) },

            { "E7FM", (e) => VerifySingleEvent(e, "Logger3", EventTypes.FormattedMessage, 7, LogLevel.Information,
                @"""FormattedMessage"":""Logger3 Event7 Information inner scope closed " + DoubleParam2.ToString() + " 37") },
            { "E7JS", (e) => VerifySingleEvent(e, "Logger3", EventTypes.MessageJson, 7, LogLevel.Information,
                        @"""ArgumentsJson"":{""stringParam"":""inner scope closed"",""doubleParam"":""" + DoubleParam2.ToString() + @""",""intParam"":""37""") },
            { "E7MSG", (e) => VerifySingleEvent(e, "Logger3", EventTypes.Message, 7, LogLevel.Information,
                @"{""Key"":""stringParam"",""Value"":""inner scope closed""}",
                @"{""Key"":""doubleParam"",""Value"":""" + DoubleParam2.ToString() +@"""}",
                @"{""Key"":""intParam"",""Value"":""37""}") },

            { "E8FM", (e) => VerifySingleEvent(e, "Logger2", EventTypes.FormattedMessage, 8, LogLevel.Warning,
                @"""FormattedMessage"":""Logger2 Event8 Warning Outer scope closed " + TimeParam.ToString("O")) },
            { "E8JS", (e) => VerifySingleEvent(e, "Logger2", EventTypes.MessageJson, 8, LogLevel.Warning,
                        @"""ArgumentsJson"":{""stringParam"":""Outer scope closed"",""timeParam"":""" + TimeParam.ToString("O")) },
            { "E8MSG", (e) => VerifySingleEvent(e, "Logger2", EventTypes.Message, 8, LogLevel.Warning,
                @"{""Key"":""stringParam"",""Value"":""Outer scope closed""}",
                @"{""Key"":""timeParam"",""Value"":""" + TimeParam.ToString("O") +@"""}") },

            { "OuterScopeJsonStart", (e) => VerifySingleEvent(e, "Logger1", EventTypes.ActivityJsonStart, null, null,
                        @"""ArgumentsJson"":{""stringParam"":""scoped foo"",""intParam"":""13"",""doubleParam"":""" + DoubleParam1.ToString()) },
            { "OuterScopeJsonStop", (e) => VerifySingleEvent(e, "Logger1", EventTypes.ActivityJsonStop, null, null) },

            { "OuterScopeStart", (e) => VerifySingleEvent(e, "Logger1", EventTypes.ActivityStart, null, null) },
            { "OuterScopeStop", (e) => VerifySingleEvent(e, "Logger1", EventTypes.ActivityStop, null, null) },

            { "InnerScopeJsonStart", (e) => VerifySingleEvent(e, "Logger3", EventTypes.ActivityJsonStart, null, null,
                        @"""ArgumentsJson"":{""timeParam"":""" + TimeParam.ToString() + @""",""guidParam"":""" + GuidParam.ToString("D")) },
            { "InnerScopeJsonStop", (e) => VerifySingleEvent(e, "Logger3", EventTypes.ActivityJsonStop, null, null) },

            { "InnerScopeStart", (e) => VerifySingleEvent(e, "Logger3", EventTypes.ActivityStart, null, null) },
            { "InnerScopeStop", (e) => VerifySingleEvent(e, "Logger3", EventTypes.ActivityStop, null, null) },
        };
    }
}
