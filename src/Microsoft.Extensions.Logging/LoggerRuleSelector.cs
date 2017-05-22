using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Logging
{
    public class LoggerRuleSelector
    {
        public void Select(LoggerFilterOptions options, string logger, string category, out LogLevel? minLevel, out LogMessageFilter filter)
        {
            minLevel = null;
            filter = null;

            // Filter rule selection:
            // 1. Select rules for current logger type, if there is none, select ones without logger type specified
            // 2. Select rules with longest matching categories
            // 3. If there no category
            // 3. If there is only one rule use it's level and filter
            // 4. If there are multiple rules combine them using AND operator
            // 5. If there are no applicable rules use global minimal level

            var loggerSpecificRules = options.Rules.Where(rule => rule.LoggerType == logger).ToList();
            if (!loggerSpecificRules.Any())
            {
                loggerSpecificRules = options.Rules.Where(rule => string.IsNullOrEmpty(rule.LoggerType)).ToList();
            }

            if (loggerSpecificRules.Any())
            {
                var categorySpecificRules = loggerSpecificRules
                    .Where(rule => !string.IsNullOrEmpty(rule.CategoryName) && category.StartsWith(rule.CategoryName, StringComparison.OrdinalIgnoreCase))
                    .GroupBy(rule => rule.CategoryName.Length)
                    .OrderByDescending(group => group.Key)
                    .FirstOrDefault()
                    ?.ToList();

                if (categorySpecificRules?.Any() != true)
                {
                    categorySpecificRules = loggerSpecificRules.Where(rule => string.IsNullOrEmpty(rule.CategoryName)).ToList();
                }

                if (!categorySpecificRules.Any())
                {
                    categorySpecificRules = loggerSpecificRules.Where(rule => rule.CategoryName.Equals("Default", StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (categorySpecificRules.Any())
                {
                    if (categorySpecificRules.Count == 1)
                    {
                        var loggerFilterRule = categorySpecificRules.Single();
                        filter = loggerFilterRule.Filter;
                        minLevel = loggerFilterRule.LogLevel;
                    }
                    else
                    {
                        // Combine rules, for min level we take maximum of all rules
                        // for filter delegated we take firs if there is only one or apply AND operator to all
                        minLevel = categorySpecificRules.Max(rule => rule.LogLevel);
                        filter = (type, c, level) =>
                        {
                            foreach (var loggerFilterRule in categorySpecificRules)
                            {
                                if (!loggerFilterRule.Filter(type, c, level))
                                {
                                    return false;
                                }
                            }

                            return true;
                        };
                    }
                }
                else
                {
                    // If there are no rules fallback to global min level
                    minLevel = options.MinLevel;
                }
            }
        }
    }
}