using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Snip
{
    public static class TimeSpanExtensions
    {
        private static readonly Dictionary<double, Func<TimeSpan, string>> relativeSpanStrings = new Dictionary<double, Func<TimeSpan, string>>()
        {
            { 0.75,              (span) => "less than a minute" },
            { 1.5,               (span) => "a minute" },
            { 45,                (span) => string.Format("{0} minutes", Math.Round(span.TotalMinutes)) },
            { 90,                (span) => "an hour" },
            { 60 * 24,           (span) => string.Format("{0} hours", Math.Round(Math.Abs(span.TotalHours))) },
            { 60 * 48,           (span) => "a day" },
            { 60 * 24 * 30,      (span) => string.Format("{0} days", Math.Floor(Math.Abs(span.TotalDays))) },
            { 60 * 24 * 60,      (span) => "a month" },
            { 60 * 24 * 365 ,    (span) => string.Format("{0} months", Math.Floor(Math.Abs(span.TotalDays / 30))) },
            { 60 * 24 * 365 * 2, (span) => "a year" },
            { double.MaxValue,   (span) => string.Format("{0} years", Math.Floor(Math.Abs(span.TotalDays / 365))) }
        };

        public static string ToRelative(this TimeSpan span)
        {
            return relativeSpanStrings.First(n => span.TotalMinutes < n.Key).Value(span) + (span.TotalMilliseconds < 0 ? " from now" : " ago");
        }
    }
}
