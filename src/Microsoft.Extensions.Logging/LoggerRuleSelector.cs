// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.Logging
{
    internal class LoggerRuleSelector
    {
        public void Select(LoggerFilterOptions options, string logger, string category, out LogLevel? minLevel, out Func<string, string, LogLevel, bool> filter)
        {
            filter = null;
            minLevel = options.MinLevel;

            var categorySpecificRules = GetMatchingRules(options, logger, category);

            var loggerFilterRule = categorySpecificRules?.LastOrDefault();
            if (loggerFilterRule != null)
            {
                filter = loggerFilterRule.Filter;
                minLevel = loggerFilterRule.LogLevel;
            }
        }

        private static List<LoggerFilterRule> GetMatchingRules(LoggerFilterOptions options, string logger, string category)
        {
            // TODO: This can be rewritten to a single loop.
            // Filter rule selection:
            // 1. Select rules for current logger type, if there is none, select ones without logger type specified
            // 2. Select rules with longest matching categories
            // 3. If there nothing matched by category take all rules without category
            // 3. If there is only one rule use it's level and filter
            // 4. If there are multiple rules use last
            // 5. If there are no applicable rules use global minimal level

            var loggerSpecificRules = options.Rules.Where(rule => rule.LoggerType == logger).ToList();
            if (!loggerSpecificRules.Any())
            {
                loggerSpecificRules = options.Rules.Where(rule => string.IsNullOrEmpty(rule.LoggerType)).ToList();
            }

            List<LoggerFilterRule> categorySpecificRules = null;
            if (loggerSpecificRules.Any())
            {
                categorySpecificRules = loggerSpecificRules
                    .Where(rule => !string.IsNullOrEmpty(rule.CategoryName) &&
                                   category.StartsWith(rule.CategoryName, StringComparison.OrdinalIgnoreCase))
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
                    categorySpecificRules = loggerSpecificRules
                        .Where(rule => rule.CategoryName.Equals("Default", StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }
            return categorySpecificRules;
        }
    }
}