using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Cyrus.Extensions.DateTimeExtensions
{
    [DebuggerDisplay("DayOfWeek={DayOfWeek},StartTime={StartTime},EndTime={EndTime}")]
    public class BusinessDay
    {
        public BusinessDay(DayOfWeek dayOfWeek) : this(dayOfWeek, TimeSpan.FromHours(9), TimeSpan.FromHours(17)) { }
        public BusinessDay(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime.TotalDays >= 1)
                throw new ArgumentOutOfRangeException(nameof(startTime), startTime, $"The {nameof(startTime)}({startTime}) argument must be less than one day.");
            if (endTime.TotalDays >= 1)
                throw new ArgumentOutOfRangeException(nameof(endTime), endTime, $"The {nameof(endTime)}({endTime}) argument must be less than one day.");
            if (startTime >= endTime)
                throw new ArgumentException($"The second argument({endTime}) must be greater than the first one({startTime}).", $"{nameof(startTime)} and {nameof(endTime)}");
            StartTime = startTime;
            EndTime = endTime;
            DayOfWeek = dayOfWeek;
        }
        public DayOfWeek DayOfWeek { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public TimeSpan StartTime { get; private set; }

        /// <summary>
        /// 判断指定System.DateTime值是否处于工作时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool IsBusinessDay(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek && date.TimeOfDay >= StartTime && date.TimeOfDay < EndTime)
                return true;
            return false;
        }

        /// <summary>
        /// 判断指定System.DateTimeOffset值是否处于工作时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool IsBusinessDay(DateTimeOffset date)
        {
            return IsBusinessDay(date.DateTime);
        }
    }
}
