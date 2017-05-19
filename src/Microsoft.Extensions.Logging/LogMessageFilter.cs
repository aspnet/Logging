namespace Microsoft.Extensions.Logging
{
    public delegate bool LogMessageFilter(string loggerType, string categoryName, LogLevel level);
}