using System;
using System.Text;

namespace Cyrus.Extensions.DateTimeExtensions
{
    public struct AgeSpan
    {
        private TimeSpan _span;
        public AgeSpan(TimeSpan span) : this()
        {
            _span = span;
        }

        public int Days { get; private set; }

        public int Hours => TimeSpan.Hours;

        public int Milliseconds => TimeSpan.Milliseconds;

        public int Minutes => TimeSpan.Minutes;

        public int Months { get; private set; }

        public int Seconds => TimeSpan.Seconds;

        public TimeSpan TimeSpan
        {
            get
            {
                return _span;
            }
            set
            {
                _span = value;
                Init(value);
            }
        }

        public double TotalDays => TimeSpan.TotalDays;

        public double TotalHours => TimeSpan.TotalHours;

        public double TotalMilliseconds => TimeSpan.TotalMilliseconds;

        public double TotalMinutes => TimeSpan.TotalMinutes;

        public double TotalMonths { get; private set; }

        public double TotalSeconds => TimeSpan.TotalSeconds;

        public double TotalWeeks { get; private set; }

        public double TotalYears { get; private set; }

        public int Weeks { get; private set; }

        public int Years { get; private set; }

        public override string ToString()
        {
            return ToString(Int32.MaxValue, false);
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="maxParts"></param>
        /// <param name="shortForm"></param>
        /// <param name="includeMilliseconds"></param>
        /// <returns></returns>
        public string ToString(int maxParts, bool shortForm = false, bool includeMilliseconds = false)
        {
            int partCount = 0;
            if (maxParts <= 0)
                maxParts = Int32.MaxValue;
            var sb = new StringBuilder();

            if (AppendPart(sb, "year", Years, shortForm, ref partCount))
                if (maxParts > 0 && partCount >= maxParts)
                    return sb.ToString();

            if (AppendPart(sb, "month", Months, shortForm, ref partCount))
                if (maxParts > 0 && partCount >= maxParts)
                    return sb.ToString();

            if (AppendPart(sb, "week", Weeks, shortForm, ref partCount))
                if (maxParts > 0 && partCount >= maxParts)
                    return sb.ToString();

            if (AppendPart(sb, "day", Days, shortForm, ref partCount))
                if (maxParts > 0 && partCount >= maxParts)
                    return sb.ToString();

            if (AppendPart(sb, "hour", Hours, shortForm, ref partCount))
                if (maxParts > 0 && partCount >= maxParts)
                    return sb.ToString();

            if (AppendPart(sb, "minute", Minutes, shortForm, ref partCount))
                if (maxParts > 0 && partCount >= maxParts)
                    return sb.ToString();

            double seconds = includeMilliseconds || Math.Abs(TotalMinutes) > 1d ? Seconds : Math.Round(TotalSeconds, 2);
            if (seconds > 10)
                seconds = Math.Round(seconds);

            if (AppendPart(sb, "second", seconds, shortForm, ref partCount))
                if (maxParts > 0 && partCount >= maxParts)
                    return sb.ToString();

            if (includeMilliseconds && AppendPart(sb, "millisecond", Milliseconds, shortForm, ref partCount))
                if (maxParts > 0 && partCount >= maxParts)
                    return sb.ToString();

            return sb.ToString();
        }

        /// <summary>
        /// 追加区间描述
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="partName"></param>
        /// <param name="partValue"></param>
        /// <param name="shortForm"></param>
        /// <param name="partCount"></param>
        /// <returns></returns>
        private static bool AppendPart(StringBuilder builder, string partName, double partValue, bool shortForm, ref int partCount)
        {
            const string spacer = " ";

            if (Math.Abs(partValue) == 0)
                return false;

            if (builder.Length > 0)
                builder.Append(spacer);

            string partValueString = partCount > 0 ? Math.Abs(partValue).ToString("0.##") : partValue.ToString("0.##");

            if (shortForm && partName == "millisecond")
                partName = "ms";
            else if (shortForm && partName == "minute")
                partName = "min";
            else if (shortForm)
                partName = partName.Substring(0, 1);

            if (shortForm)
                builder.AppendFormat("{0}{1}", partValueString, partName);
            else
                builder.AppendFormat("{0} {1}{2}", partValueString, partName, GetTense(partValue));
            partCount++;

            return true;
        }

        /// <summary>
        /// 返回复数后缀's'（当参数大于1时）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string GetTense(double value)
        {
            return Math.Abs(value) > 1 ? "s" : String.Empty;
        }

        private void Init(TimeSpan span)
        {
            TotalYears = span.GetTotalYears();
            Years = span.GetYears();
            TotalMonths = span.GetTotalMonths();
            Months = span.GetMonths();
            TotalWeeks = span.GetTotalWeeks();
            Weeks = span.GetWeeks();
            Days = span.GetDays();
        }
    }

    public static class TimeSpanExtensions
    {
        /// <summary>
        /// 每月平均天数
        /// </summary>
        public const double AvgDaysInAMonth = 30.436875d;

        /// <summary>
        /// 每年平均天数
        /// </summary>
        public const double AvgDaysInAYear = 365.2425d;

        /// <summary>
        /// 向上舍弃区间精度
        /// </summary>
        /// <param name="time"></param>
        /// <param name="roundingInterval"></param>
        /// <returns></returns>
        public static TimeSpan Ceiling(this TimeSpan time, TimeSpan roundingInterval)
        {
            return new TimeSpan(Convert.ToInt64(Math.Ceiling(time.Ticks / (double)roundingInterval.Ticks)) * roundingInterval.Ticks);
        }

        /// <summary>
        /// 向下舍弃区间精度
        /// </summary>
        /// <param name="time"></param>
        /// <param name="roundingInterval"></param>
        /// <returns></returns>
        public static TimeSpan Floor(this TimeSpan time, TimeSpan roundingInterval)
        {
            return new TimeSpan(Convert.ToInt64(Math.Floor(time.Ticks / (double)roundingInterval.Ticks)) * roundingInterval.Ticks);
        }

        /// <summary>
        /// 返回当前实例不够一整周的剩余天数
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static int GetDays(this TimeSpan timespan)
        {
            return (int)(timespan.TotalDays % 7d);
        }

        /// <summary>
        /// 返回当前实例不够一整年的剩余月数
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static int GetMonths(this TimeSpan timespan)
        {
            return (int)((timespan.TotalDays % AvgDaysInAYear) / AvgDaysInAMonth);
        }

        /// <summary>
        /// 返回当前实例的总微秒数
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static double GetTotalMicroseconds(this TimeSpan span)
        {
            return span.Ticks / 10d;
        }

        /// <summary>
        /// 返回当前实例的总月数
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static double GetTotalMonths(this TimeSpan timespan)
        {
            return timespan.TotalDays / AvgDaysInAMonth;
        }

        /// <summary>
        /// 返回当前实例的总纳秒数
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static double GetTotalNanoseconds(this TimeSpan span)
        {
            return span.Ticks * 100d;
        }
        /// <summary>
        /// 返回当前实例的总周数
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static double GetTotalWeeks(this TimeSpan timespan)
        {
            return timespan.TotalDays / 7d;
        }

        /// <summary>
        /// 返回当前实例的总年数
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static double GetTotalYears(this TimeSpan timespan)
        {
            return timespan.TotalDays / AvgDaysInAYear;
        }

        /// <summary>
        /// 返回当前实例不够一整月的剩余周数
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static int GetWeeks(this TimeSpan timespan)
        {
            return (int)(((timespan.TotalDays % AvgDaysInAYear) % AvgDaysInAMonth) / 7d);
        }

        /// <summary>
        /// 返回当前实例的整年数
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        public static int GetYears(this TimeSpan timespan)
        {
            return (int)(timespan.TotalDays / AvgDaysInAYear);
        }

        /// <summary>
        /// 舍弃区间精度（需要取近似值时向上或向下取最近的偶数值）
        /// </summary>
        /// <param name="time"></param>
        /// <param name="roundingInterval"></param>
        /// <param name="roundingType"></param>
        /// <returns></returns>
        public static TimeSpan Round(this TimeSpan time, TimeSpan roundingInterval, MidpointRounding roundingType = MidpointRounding.ToEven)
        {
            return new TimeSpan(Convert.ToInt64(Math.Round(time.Ticks / (double)roundingInterval.Ticks, roundingType)) * roundingInterval.Ticks);
        }

        /// <summary>
        /// 返回当前实例所表示的AgeSpan
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        public static AgeSpan ToAgeSpan(this TimeSpan span)
        {
            return new AgeSpan(span);
        }

        /// <summary>
        /// 返回当前实例所表示的AgeSpan指定[长|短]的精确描述（秒）
        /// </summary>
        /// <param name="span"></param>
        /// <param name="shortForm"></param>
        /// <returns></returns>
        public static string ToWords(this TimeSpan span, bool shortForm = false)
        {
            return ToWords(span, -1, shortForm);
        }

        /// <summary>
        /// 返回当前实例所表示的AgeSpan指定精度和[长|短]的近似描述
        /// </summary>
        /// <param name="span"></param>
        /// <param name="maxParts"></param>
        /// <param name="shortForm"></param>
        /// <returns></returns>
        public static string ToWords(this TimeSpan span, int maxParts, bool shortForm = false)
        {
            var age = new AgeSpan(span);
            return age.ToString(maxParts, shortForm);
        }
    }
}
