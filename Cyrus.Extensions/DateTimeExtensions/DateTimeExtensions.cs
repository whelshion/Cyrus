using System;

namespace Cyrus.Extensions.DateTimeExtensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// UNIX纪元初始值（1970-01-01 00:00:00.000）的TICKS值
        /// 1(TICK)=100(ns)
        /// </summary>
        private const long EPOCH_TICKS = 621355968000000000;

        /// <summary>
        /// Returns a new System.DateTime that adds the specified number of weeks to the value of this instance.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value">A number of weeks. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of weeks represented by value.</returns>
        public static DateTime AddWeeks(this DateTime date, double value)
        {
            return date.SafeAdd(TimeSpan.FromDays(value * 7));
        }

        /// <summary>
        /// 向上舍弃区间精度
        /// （ps:TimeSpan.FromDays(7)可获取当前实例的下周一）
        /// </summary>
        /// <param name="date"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static DateTime Ceiling(this DateTime date, TimeSpan interval)
        {
            return date.AddTicks(interval.Ticks - (date.Ticks % interval.Ticks));
        }

        /// <summary>
        /// 修改当前实例各区间的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static DateTime Change(this DateTime date, int? year = null, int? month = null, int? day = null, int? hour = null, int? minute = null, int? second = null)
        {
            var result = date;

            if (year.HasValue)
                result = result.ChangeYear(year.Value);
            if (month.HasValue)
                result = result.ChangeMonth(month.Value);
            if (day.HasValue)
                result = result.ChangeDay(day.Value);
            if (hour.HasValue)
                result = result.ChangeHour(hour.Value);
            if (minute.HasValue)
                result = result.ChangeMinute(minute.Value);
            if (second.HasValue)
                result = result.ChangeSecond(second.Value);

            return result;
        }

        /// <summary>
        /// 修改当前实例日区间的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime ChangeDay(this DateTime date, int day)
        {
            if (day < 1 || day > 31)
                throw new ArgumentException("Value must be between 1 and 31.", nameof(day));

            if (day > DateTime.DaysInMonth(date.Year, date.Month))
                throw new ArgumentException("Value must be a valid source.", nameof(day));

            return date.AddDays(day - date.Day);
        }

        /// <summary>
        /// 修改当前实例小时区间的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        public static DateTime ChangeHour(this DateTime date, int hour)
        {
            if (hour < 0 || hour > 23)
                throw new ArgumentException("Value must be between 0 and 23.", nameof(hour));

            return date.AddHours(hour - date.Hour);
        }

        /// <summary>
        /// 修改当前实例毫秒区间的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="millisecond"></param>
        /// <returns></returns>
        public static DateTime ChangeMillisecond(this DateTime date, int millisecond)
        {
            if (millisecond < 0 || millisecond > 59)
                throw new ArgumentException("Value must be between 0 and 999.", nameof(millisecond));

            return date.AddMilliseconds(millisecond - date.Millisecond);
        }

        /// <summary>
        /// 修改当前实例分区间的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static DateTime ChangeMinute(this DateTime date, int minute)
        {
            if (minute < 0 || minute > 59)
                throw new ArgumentException("Value must be between 0 and 59.", nameof(minute));

            return date.AddMinutes(minute - date.Minute);
        }

        /// <summary>
        /// 修改当前实例月区间的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static DateTime ChangeMonth(this DateTime date, int month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentException("Value must be between 1 and 12.", nameof(month));

            return date.AddMonths(month - date.Month);
        }

        /// <summary>
        /// 修改当前实例秒区间的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static DateTime ChangeSecond(this DateTime date, int second)
        {
            if (second < 0 || second > 59)
                throw new ArgumentException("Value must be between 0 and 59.", nameof(second));

            return date.AddSeconds(second - date.Second);
        }

        /// <summary>
        /// 修改当前实例年区间的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime ChangeYear(this DateTime date, int year)
        {
            return date.AddYears(year - date.Year);
        }

        /// <summary>
        /// 返回当前实例所处天的结束时刻
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndOfDay(this DateTime date)
        {
            var value = date.StartOfDay().SafeAdd(TimeSpan.FromDays(1));
            if (value == DateTime.MaxValue)
                return value;

            return value.SubtractMilliseconds(1);
        }

        /// <summary>
        /// 返回当前实例所处小时的结束时刻
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndOfHour(this DateTime date)
        {
            var value = date.StartOfHour().SafeAdd(TimeSpan.FromHours(1));
            if (value == DateTime.MaxValue)
                return value;

            return value.SubtractMilliseconds(1);
        }

        /// <summary>
        /// 返回当前实例所处分钟的结束时刻
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndOfMinute(this DateTime date)
        {
            var value = date.StartOfMinute().SafeAdd(TimeSpan.FromMinutes(1));
            if (value == DateTime.MaxValue)
                return value;

            return value.SubtractMilliseconds(1);
        }

        /// <summary>
        /// 返回当前实例所处月份的结束时刻
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndOfMonth(this DateTime date)
        {
            var value = date.StartOfMonth().AddMonths(1);
            if (value == DateTime.MaxValue)
                return value;

            return value.SubtractMilliseconds(1);
        }

        /// <summary>
        /// 返回当前实例所处秒的结束时刻
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndOfSecond(this DateTime date)
        {
            var value = date.StartOfSecond().SafeAdd(TimeSpan.FromSeconds(1));
            if (value == DateTime.MaxValue)
                return value;

            return value.SubtractMilliseconds(1);
        }

        /// <summary>
        /// 返回当前实例所处周的结束时刻
        /// </summary>
        /// <param name="date"></param>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTime EndOfWeek(this DateTime date, DayOfWeek startOfWeek = DayOfWeek.Sunday)
        {
            var value = date.StartOfWeek(startOfWeek).AddWeeks(1);
            if (value == DateTime.MaxValue)
                return value;

            return value.SubtractMilliseconds(1);
        }

        /// <summary>
        /// 返回当前实例所处年的结束时刻
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndOfYear(this DateTime date)
        {
            var value = date.StartOfYear().AddYears(1);
            if (value == DateTime.MaxValue)
                return value;

            return value.SubtractMilliseconds(1);
        }

        /// <summary>
        /// 向下舍弃区间精度
        /// （ps:TimeSpan.FromDays(7)可获取当前实例所处周的周一）
        /// </summary>
        /// <param name="date"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static DateTime Floor(this DateTime date, TimeSpan interval)
        {
            return date.AddTicks(-(date.Ticks % interval.Ticks));
        }

        /// <summary>
        /// 返回当前实例距离此刻所表示的AgeSpan
        /// </summary>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        public static AgeSpan GetAge(this DateTime fromDate)
        {
            return GetAge(fromDate, DateTime.Now);
        }

        /// <summary>
        /// 返回当前实例距离指定值所表示的AgeSpan
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static AgeSpan GetAge(this DateTime fromDate, DateTime toDate)
        {
            return new AgeSpan(toDate - fromDate);
        }

        /// <summary>
        /// 判断当前实例是否处于两个指定值之间（含相等）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start">开始时刻（不大于结束时刻）</param>
        /// <param name="end">结束时刻（不小于开始时刻）</param>
        /// <returns></returns>
        public static bool Intersects(this DateTime source, DateTime start, DateTime end)
        {
            if (start.CompareTo(end) > 0)
                throw new ArgumentException($"The second argument({end.ToString()}) can't be less than the first one({start.ToString()}).", $"{nameof(start)} and {nameof(end)}");
            return source >= start && source <= end;
        }

        /// <summary>
        /// 判断当前实例是否处于指定值所在的日期
        /// </summary>
        /// <param name="source"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IntersectsDay(this DateTime source, DateTime date)
        {
            return source.Intersects(date.StartOfDay(), date.EndOfDay());
        }

        /// <summary>
        /// 判断当前实例是否处于指定值所在的小时
        /// </summary>
        /// <param name="source"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IntersectsHour(this DateTime source, DateTime date)
        {
            return source.Intersects(date.StartOfHour(), date.EndOfHour());
        }

        /// <summary>
        /// 判断当前实例是否处于指定值所在的分钟
        /// </summary>
        /// <param name="source"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IntersectsMinute(this DateTime source, DateTime date)
        {
            return source.Intersects(date.StartOfMinute(), date.EndOfMinute());
        }

        /// <summary>
        /// 判断当前实例是否处于指定值所在的月份
        /// </summary>
        /// <param name="source"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IntersectsMonth(this DateTime source, DateTime date)
        {
            return source.Intersects(date.StartOfMonth(), date.EndOfMonth());
        }

        /// <summary>
        /// 判断当前实例是否处于指定值所在的秒
        /// </summary>
        /// <param name="source"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IntersectsSecond(this DateTime source, DateTime date)
        {
            return source.Intersects(date.StartOfSecond(), date.EndOfSecond());
        }

        /// <summary>
        /// 判断当前实例是否处于指定值所在的年份
        /// </summary>
        /// <param name="source"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IntersectsYear(this DateTime source, DateTime date)
        {
            return source.Intersects(date.StartOfYear(), date.EndOfYear());
        }

        /// <summary>
        /// 判断当前实例是否晚于指定值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsAfter(this DateTime date, DateTime value)
        {
            return date > value;
        }

        /// <summary>
        /// 判断当前实例是否不早于指定值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsAfterOrEqual(this DateTime date, DateTime value)
        {
            return date >= value;
        }

        /// <summary>
        /// 判断当前实例是否早于指定值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsBefore(this DateTime date, DateTime value)
        {
            return date < value;
        }

        /// <summary>
        /// 判断当前实例是否不晚于指定值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsBeforeOrEqual(this DateTime date, DateTime value)
        {
            return date <= value;
        }

        /// <summary>
        /// 返回当前实例一天前的值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime LastDay(this DateTime date)
        {
            return date.SafeSubtract(TimeSpan.FromDays(1));
        }

        /// <summary>
        /// 返回当前实例一小时前的值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime LastHour(this DateTime date)
        {
            return date.SafeSubtract(TimeSpan.FromHours(1));
        }

        /// <summary>
        /// 返回当前实例一分钟前的值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime LastMinute(this DateTime date)
        {
            return date.SafeSubtract(TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// 返回当前实例一个月前的值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime LastMonth(this DateTime date)
        {
            return date.SubtractMonths(1);
        }

        /// <summary>
        /// 返回当前实例一秒前的值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime LastSecond(this DateTime date)
        {
            return date.SafeSubtract(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// 返回当前实例一周前的值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime LastWeek(this DateTime date)
        {
            return date.SubtractWeeks(1);
        }

        /// <summary>
        /// 返回当前实例一年前的值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime LastYear(this DateTime date)
        {
            return date.SubtractYears(1);
        }

        /// <summary>
        /// 返回当前实例一天后的值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime NextDay(this DateTime date)
        {
            return date.SafeAdd(TimeSpan.FromDays(1));
        }

        /// <summary>
        /// 返回当前实例一小时后的值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime NextHour(this DateTime date)
        {
            return date.SafeAdd(TimeSpan.FromHours(1));
        }

        /// <summary>
        /// 返回当前实例一分钟后的值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime NextMinute(this DateTime date)
        {
            return date.SafeAdd(TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// 返回当前实例一个月后的值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime NextMonth(this DateTime date)
        {
            return date.AddMonths(1);
        }

        /// <summary>
        /// 返回当前实例一秒后的值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime NextSecond(this DateTime date)
        {
            return date.SafeAdd(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// 返回当前实例一周后的值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime NextWeek(this DateTime date)
        {
            return date.AddWeeks(1);
        }

        /// <summary>
        /// 返回当前实例一年后的值
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime NextYear(this DateTime date)
        {
            return date.AddYears(1);
        }

        /// <summary>
        /// 舍弃区间精度（四舍五入）
        /// </summary>
        /// <param name="date"></param>
        /// <param name="roundingInterval"></param>
        /// <returns></returns>
        public static DateTime Round(this DateTime date, TimeSpan roundingInterval)
        {
            long halfIntervalTicks = ((roundingInterval.Ticks + 1) >> 1);
            return date.AddTicks(halfIntervalTicks - ((date.Ticks + halfIntervalTicks) % roundingInterval.Ticks));
        }

        /// <summary>
        /// 返回当前实例向后安全偏移指定偏移量后的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime SafeAdd(this DateTime date, TimeSpan value)
        {
            if (date.Ticks + value.Ticks < DateTime.MinValue.Ticks)
                return DateTime.MinValue;

            if (date.Ticks + value.Ticks > DateTime.MaxValue.Ticks)
                return DateTime.MaxValue;

            return date.Add(value);
        }

        /// <summary>
        /// 返回当前实例向前安全偏移指定偏移量后的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime SafeSubtract(this DateTime date, TimeSpan value)
        {
            if (date.Ticks - value.Ticks < DateTime.MinValue.Ticks)
                return DateTime.MinValue;

            if (date.Ticks - value.Ticks > DateTime.MaxValue.Ticks)
                return DateTime.MaxValue;

            return date.Subtract(value);
        }

        /// <summary>
        /// 返回当前实例所处天的开始时刻
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime StartOfDay(this DateTime date)
        {
            return date.Floor(TimeSpan.FromDays(1));
        }

        /// <summary>
        /// 返回当前实例所处小时的开始时刻
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime StartOfHour(this DateTime date)
        {
            return date.Floor(TimeSpan.FromHours(1));
        }

        /// <summary>
        /// 返回当前实例所处分钟的开始时刻
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime StartOfMinute(this DateTime date)
        {
            return date.Floor(TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// 返回当前实例所处月份的开始时刻
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime StartOfMonth(this DateTime date)
        {
            return date.StartOfDay().SafeSubtract(TimeSpan.FromDays(date.Date.Day - 1));
        }

        /// <summary>
        /// 返回当前实例所处秒的开始时刻
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime StartOfSecond(this DateTime date)
        {
            return date.Floor(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// 返回当前实例所处周的开始时刻
        /// </summary>
        /// <param name="date"></param>
        /// <param name="startOfWeek">周起始天，默认星期天</param>
        /// <returns></returns>
        public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeek = DayOfWeek.Sunday)
        {
            int diff = date.DayOfWeek - startOfWeek;
            if (diff < 0)
                diff += 7;

            return date.StartOfDay().SafeSubtract(TimeSpan.FromDays(diff));
        }

        /// <summary>
        /// 返回当前实例所处年的开始时刻
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime StartOfYear(this DateTime date)
        {
            return date.StartOfMonth().SubtractMonths(date.Date.Month - 1);
        }

        /// <summary>
        /// 返回当前实例向前安全偏移指定天数的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value">大于0的双精度值</param>
        /// <returns></returns>
        public static DateTime SubtractDays(this DateTime date, double value)
        {
            if (value < 0)
                throw new ArgumentException("Value cannot be less than 0.", nameof(value));

            return date.SafeSubtract(TimeSpan.FromDays(value));
        }

        /// <summary>
        /// 返回当前实例向前安全偏移指定小时数的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value">大于0的双精度值</param>
        /// <returns></returns>
        public static DateTime SubtractHours(this DateTime date, double value)
        {
            if (value < 0)
                throw new ArgumentException("Value cannot be less than 0.", nameof(value));

            return date.SafeSubtract(TimeSpan.FromHours(value));
        }

        /// <summary>
        /// 返回当前实例向前安全偏移指定毫秒数的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value">大于0的双精度值</param>
        /// <returns></returns>
        public static DateTime SubtractMilliseconds(this DateTime date, double value)
        {
            if (value < 0)
                throw new ArgumentException("Value cannot be less than 0.", nameof(value));

            return date.SafeSubtract(TimeSpan.FromMilliseconds(value));
        }

        /// <summary>
        /// 返回当前实例向前安全偏移指定分钟数的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value">大于0的双精度值</param>
        /// <returns></returns>
        public static DateTime SubtractMinutes(this DateTime date, double value)
        {
            if (value < 0)
                throw new ArgumentException("Value cannot be less than 0.", nameof(value));

            return date.SafeSubtract(TimeSpan.FromMinutes(value));
        }

        /// <summary>
        /// 返回当前实例向前安全偏移指定月份数的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="months">大于0的整型值</param>
        /// <returns></returns>
        public static DateTime SubtractMonths(this DateTime date, int months)
        {
            if (months < 0)
                throw new ArgumentException("Months cannot be less than 0.", nameof(months));

            return date.AddMonths(months * -1);
        }

        /// <summary>
        /// 返回当前实例向前安全偏移指定秒数的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value">大于0的双精度值</param>
        /// <returns></returns>
        public static DateTime SubtractSeconds(this DateTime date, double value)
        {
            if (value < 0)
                throw new ArgumentException("Value cannot be less than 0.", nameof(value));

            return date.SafeSubtract(TimeSpan.FromSeconds(value));
        }

        /// <summary>
        /// 返回当前实例向前安全偏移指定计时周期数的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value">大于0的长整型值</param>
        /// <returns></returns>
        public static DateTime SubtractTicks(this DateTime date, long value)
        {
            if (value < 0)
                throw new ArgumentException("Value cannot be less than 0.", nameof(value));

            return date.SafeSubtract(TimeSpan.FromTicks(value));
        }

        /// <summary>
        /// 返回当前实例向前安全偏移指定周数的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value">大于0的双精度值</param>
        /// <returns></returns>
        public static DateTime SubtractWeeks(this DateTime date, double value)
        {
            if (value < 0)
                throw new ArgumentException("Value cannot be less than 0.", nameof(value));

            return date.SafeSubtract(TimeSpan.FromDays(value * 7));
        }

        /// <summary>
        /// 返回当前实例向前安全偏移指定年份数的值
        /// </summary>
        /// <param name="date"></param>
        /// <param name="value">大于0的整型值</param>
        /// <returns></returns>
        public static DateTime SubtractYears(this DateTime date, int value)
        {
            if (value < 0)
                throw new ArgumentException("Value cannot be less than 0.", nameof(value));

            return date.AddYears(value * -1);
        }

        /// <summary>
        /// 返回当前实例距离此刻的精确描述（秒）
        /// </summary>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        public static string ToAgeString(this DateTime fromDate)
        {
            return ToAgeString(fromDate, DateTime.Now, 0);
        }

        /// <summary>
        /// 返回当前实例距离此刻指定精度和[长|短]的描述
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="maxSpans"></param>
        /// <param name="shortForm"></param>
        /// <returns></returns>
        public static string ToAgeString(this DateTime fromDate, int maxSpans, bool shortForm = false)
        {
            return ToAgeString(fromDate, DateTime.Now, maxSpans, shortForm);
        }

        /// <summary>
        /// 返回当前实例距离指定时刻的指定精度和[长|短]的描述
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="maxSpans"></param>
        /// <param name="shortForm"></param>
        /// <returns></returns>
        public static string ToAgeString(this DateTime fromDate, DateTime toDate, int maxSpans, bool shortForm = false)
        {
            var age = GetAge(fromDate, toDate);
            return age.ToString(maxSpans, shortForm);
        }

        /// <summary>
        /// 返回当前实例距离此刻的近似描述（以最大区间）
        /// </summary>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        public static string ToApproximateAgeString(this DateTime fromDate)
        {
            bool isFuture = fromDate > DateTime.Now;
            var age = isFuture ? GetAge(DateTime.Now, fromDate) : GetAge(fromDate);
            if (age.TotalMinutes <= 1d)
                return age.TotalSeconds > 0 ? "Just now" : "Right now";

            return isFuture ? $"{age.ToString(1)} from now" : $"{age.ToString(1)} ago";
        }

        /// <summary>
        /// 返回UNIX纪元（毫秒）表示的System.DateTime（Local）
        /// </summary>
        /// <param name="milliSecondsSinceEpoch"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long milliSecondsSinceEpoch)
        {
            return new DateTime(EPOCH_TICKS + (milliSecondsSinceEpoch * TimeSpan.TicksPerMillisecond), DateTimeKind.Utc).ToLocalTime();
        }

        /// <summary>
        /// 返回当前实例所表示的System.DateTimeOffset
        /// （ps:DateTimeKind.Unspecified以Local处理）
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTimeOffset ToDateTimeOffset(this DateTime date)
        {
            TimeSpan offset = date.Kind == DateTimeKind.Utc ? TimeSpan.Zero
                                : TimeZoneInfo.Local.GetUtcOffset(date);
            return new DateTimeOffset(date, offset);
        }

        /// <summary>
        /// 返回当前实例的UNIX纪元（秒）
        /// </summary>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        public static long ToEpoch(this DateTime fromDate)
        {
            long utc = (fromDate.ToUniversalTime().Ticks - EPOCH_TICKS) / TimeSpan.TicksPerSecond;
            return utc;
        }

        /// <summary>
        /// 返回当前实例偏移指定秒数后的UNIX纪元（秒）
        /// </summary>
        /// <param name="date"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static long ToEpoch(this DateTime date, long offset)
        {
            return offset + date.ToEpoch();
        }

        /// <summary>
        /// 返回当前实例距离指定UNIX纪元（秒）的秒数
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static long ToEpochOffset(this DateTime date, long timestamp)
        {
            return timestamp - date.ToEpoch();
        }

        /// <summary>
        /// Returns the number of milliseconds that have elapsed since 1970-01-01T00:00:00.000Z.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>The number of milliseconds that have elapsed since 1970-01-01T00:00:00.000Z.</returns>
        public static long ToUnixTimeMilliseconds(this DateTime date)
        {
            return ToDateTimeOffset(date).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Returns the number of seconds that have elapsed since 1970-01-01T00:00:00Z.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>The number of seconds that have elapsed since 1970-01-01T00:00:00Z.</returns>
        public static long ToUnixTimeSeconds(this DateTime date)
        {
            return ToDateTimeOffset(date).ToUnixTimeSeconds();
        }

        /// <summary>
        /// 返回UNIX时间戳（毫秒）所表示的System.DateTime（Local）
        /// </summary>
        /// <param name="unixTimeMilliseconds"></param>
        /// <returns></returns>
        public static DateTime UnixTimeMillisecondsToDateTime(this long unixTimeMilliseconds)
        {
            return new DateTime(EPOCH_TICKS + (unixTimeMilliseconds * TimeSpan.TicksPerMillisecond), DateTimeKind.Utc).ToLocalTime();
        }

        /// <summary>
        /// 返回UNIX时间戳（秒）所表示的System.DateTime（Local）
        /// </summary>
        /// <param name="unixTimeSeconds"></param>
        /// <returns></returns>
        public static DateTime UnixTimeSecondsToDateTime(this long unixTimeSeconds)
        {
            return new DateTime(EPOCH_TICKS + unixTimeSeconds * TimeSpan.TicksPerSecond, DateTimeKind.Utc).ToLocalTime();
        }
    }
}