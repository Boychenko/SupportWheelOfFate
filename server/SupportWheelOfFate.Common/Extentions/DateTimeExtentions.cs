using System;
using System.Globalization;

namespace SupportWheelOfFate.Common.Extentions
{
    public static class DateTimeExtentions
    {
        const string DateQueryString = "yyyy-MM-dd";

        public static bool IsWeekDay(this DateTime value)
        {
            return !(value.DayOfWeek == DayOfWeek.Saturday || value.DayOfWeek == DayOfWeek.Sunday);
        }

        public static string ToQueryParam(this DateTime value)
        {
            return value.ToString(DateQueryString);
        }

        public static DateTime? DateFromQueryParam(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var datetimeStyles = DateTimeStyles.AllowLeadingWhite
                | DateTimeStyles.AllowTrailingWhite
                | DateTimeStyles.AssumeUniversal;
            if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, datetimeStyles, out var result))
            {
                return result;
            }

            return null;
        }
    }
}
