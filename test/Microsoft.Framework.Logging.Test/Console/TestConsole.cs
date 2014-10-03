using System;
using Microsoft.Framework.Logging.Console;

namespace Microsoft.Framework.Logging.Test.Console
{
    public class TestConsole : IConsole
    {
        private ConsoleColor _backgroundColor;
        private ConsoleColor _foregroundColor;
        private ConsoleSink _sink;
        
        public TestConsole(ConsoleSink sink)
        {
            _sink = sink;
        }

        public ConsoleColor BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }

            set
            {
                _backgroundColor = value;
            }
        }

        public ConsoleColor ForegroundColor
        {
            get
            {
                return _foregroundColor;
            }

            set
            {
                _foregroundColor = value;
            }
        }

        public void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            var message = string.Format(format, arg0, arg1, arg2);
            _sink.Write(new ConsoleContext()
            {
                ForegroundColor = _foregroundColor,
                BackgroundColor = _backgroundColor,
                Message = message
            });
        }
    }
}