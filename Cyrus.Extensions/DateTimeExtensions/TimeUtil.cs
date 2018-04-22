using System;

namespace Cyrus.Extensions.DateTimeExtensions
{
    public static class TimeUnit
    {
        /// <summary>
        /// 将指定字符串转换为System.TimeSpan
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TimeSpan Parse(string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            return ParseTime(value);
        }

        /// <summary>
        /// 尝试将指定字符串转换为System.TimeSpan
        /// </summary>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool TryParse(string value, out TimeSpan time)
        {
            try
            {
                if (String.IsNullOrEmpty(value))
                    return false;

                time = ParseTime(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static TimeSpan ParseTime(string value)
        {
            // compare using the original value as uppercase M could mean months.
            var normalized = value.ToLowerInvariant().Trim();
            if (value.EndsWith("m"))
            {
                int minutes = Int32.Parse(normalized.Substring(0, normalized.Length - 1));
                return new TimeSpan(0, minutes, 0);
            }

            if (normalized.EndsWith("h"))
            {
                int hours = Int32.Parse(normalized.Substring(0, normalized.Length - 1));
                return new TimeSpan(hours, 0, 0);
            }

            if (normalized.EndsWith("d"))
            {
                int days = Int32.Parse(normalized.Substring(0, normalized.Length - 1));
                return new TimeSpan(days, 0, 0, 0);
            }

            if (normalized.EndsWith("nanos"))
            {
                long nanoseconds = Int64.Parse(normalized.Substring(0, normalized.Length - 5));
                return new TimeSpan((int)Math.Round(nanoseconds / 100d));
            }

            if (normalized.EndsWith("micros"))
            {
                long microseconds = Int64.Parse(normalized.Substring(0, normalized.Length - 6));
                return new TimeSpan(microseconds * 10);
            }

            if (normalized.EndsWith("ms"))
            {
                int milliseconds = Int32.Parse(normalized.Substring(0, normalized.Length - 2));
                return new TimeSpan(0, 0, 0, 0, milliseconds);
            }

            if (normalized.EndsWith("s"))
            {
                int seconds = Int32.Parse(normalized.Substring(0, normalized.Length - 1));
                return new TimeSpan(0, 0, seconds);
            }

            throw new ArgumentException($"Unable to parse value '{value}' as a valid timespan value.");
        }
    }
}