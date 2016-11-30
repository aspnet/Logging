using Microsoft.Extensions.Logging;

namespace ConsoleHierarchySample
{
    /// <summary>
    /// Sample generic logging event IDs. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// NOTE: This enum is provided as an example only; it is recommended that each applications
    /// use their own custom logging event IDs.
    /// </para>
    /// <para>
    /// The structure of the event IDs are based on a structure similar to the theory of 
    /// response codes used for Internet systems such as SMTP and HTTP.
    /// </para>
    /// <para>
    /// In this case; the event IDs are based on a 4 digit system.
    /// </para>
    /// <para>
    /// The first digit is the class of the event: 
    /// 1 for startup code; 2 for events that have occurred (completed); 
    /// 3 for actions the program initiates; 4 for warnings; 5 for errors; 8 for shutdown or
    /// end events; and 9 for unknown or critical issues.
    /// </para>
    /// <para>
    /// The class is similar to the event level (e.g. events starting with 5 will usually
    /// be logged at the Error level).
    /// </para>
    /// <para>
    /// The second digit is the area of the program: 0 for configuration or syntax; 1 for
    /// system level; 2 for connnections; 3 for security and authentication; and 9 for 
    /// an unknown area; such as an unhandled general exception.
    /// </para>
    /// <para>
    /// Applications can use the second digit 4-8 for the major subsystems; which will
    /// vary between applications.
    /// </para>
    /// </remarks>
    public static class LoggingEvents
    {
        public static EventId ConfigurationStart = 1000;
        public static EventId SystemStart = 1100;
        public static EventId ConnectionStart = 1200;

        public static EventId SystemEvent = 2100;
        public static EventId ConnectionEvent = 2200;
        public static EventId AuthenticationSuccess = 2300;

        public static EventId ConfigurationAction = 3000;
        public static EventId SystemAction = 3100;
        public static EventId ConnectionAction = 3200;

        public static EventId ConfigurationWarning = 4000;
        public static EventId SystemWarning = 4100;
        public static EventId ConnectionWarning = 4200;
        public static EventId AuthenticationFailure = 4300;

        public static EventId ConfigurationError = 5000;
        public static EventId SystemError = 5100;
        public static EventId ConnectionError = 5200;
        public static EventId AuthenticationError = 5300;
        public static EventId UnknownError = 5900;

        public static EventId SystemStop = 8100;
        public static EventId ConnectionStop = 8200;

        public static EventId ConfigurationCriticalError = 9000;
        public static EventId SystemCriticalError = 9100;
        public static EventId ConnectionCriticalError = 9200;
        public static EventId AuthenticationCriticalError = 9300;
        public static EventId UnknownCriticalError = 9900;
    }
}
