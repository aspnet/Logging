using System;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Logging.Console
{
    /// <summary>
    /// Settings for a <see cref="ConsoleLoggerProvider"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When <see cref="IConsoleLoggerSettings"/> are defined, then only the categories 
    /// they return a result from <c>TryGetSwitch</c>, either for themselves or for a parent, 
    /// are logged.
    /// </para>
    /// <para>
    /// If a category is not configured then the value of the special category 'Default' (case sensitive) 
    /// is used, or 'Default' is not specified then they are not logged at all. 
    /// </para>
    /// </remarks>
    public interface IConsoleLoggerSettings
    {
        /// <summary>
        /// Gets whether log scope information should be displayed in the output.
        /// </summary>
        bool IncludeScopes { get; }

        IChangeToken ChangeToken { get; }

        /// <summary>
        /// Gets the minimum log level for the specified category (or the special category 'Default'),
        /// or returns false if it is not defined.
        /// </summary>
        /// <param name="name">The configuration category to check.</param>
        /// <param name="level">The minimum <c>LogLevel</c> set for the category.</param>
        /// <returns>true if a <c>LogLevel</c> is set; false if it is not.</returns>
        /// <remarks>
        /// <para>
        /// Note that the console logger calls this for a category and all parent categories until
        /// a value is returned. If no value is returned, then the value for the special category 
        /// 'Default' is checked, and if that returns false then the category is not logged.
        /// </para>
        /// </remarks>
        bool TryGetSwitch(string name, out LogLevel level);

        IConsoleLoggerSettings Reload();
    }
}
