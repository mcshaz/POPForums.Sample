using System;
using System.Collections.Generic;
using System.Text;

namespace EmailForumUpdates.Extensions
{
    public static class Dates
    {
        public static string ToLongAgoString(this DateTime utcPrior, DateTime? utcNow = null, string describeAgo = "ago")
        {
            var interval = (utcNow ?? DateTime.UtcNow) - utcPrior;
            if (interval.TotalSeconds < 60.0)
            {
                return TruncPleuralize(interval.TotalSeconds, "second", describeAgo);
            }
            if (interval.TotalMinutes < 60.0)
            {
                return TruncPleuralize(interval.TotalMinutes, "minute", describeAgo);
            }
            if (interval.TotalHours < 24.0)
            {
                return TruncPleuralize(interval.TotalHours, "hour", describeAgo);
            }
            if (interval.TotalDays < 14.0)
            {
                return TruncPleuralize(interval.TotalDays, "day", describeAgo);
            }
            double weeks = interval.TotalDays / 7.0;
            return TruncPleuralize(weeks, "week", describeAgo);
        }

        private static string TruncPleuralize(double val, string singular, string describeAgo)
        {
            var valStr = val.ToString("F0");
            return valStr + ' ' + (valStr == "1" ? singular : (singular + 's')) + ' ' + describeAgo;
        }
    }
}
