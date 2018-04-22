using System;
using System.Collections.Generic;
using System.Linq;

namespace Cyrus.Extensions.DateTimeExtensions
{
    public class BusinessWeek
    {
        private Dictionary<DayOfWeek, IList<BusinessDay>> _dayTree;

        public BusinessWeek()
        {
            BusinessDays = new List<BusinessDay>();
        }
        public BusinessWeek(string instanceName) : this()
        {
            if (instanceName != null)
                InstanceName = instanceName;
        }
        public BusinessWeek(string instanceName, IList<BusinessDay> businessDays) : this(instanceName)
        {
            if (businessDays != null)
                BusinessDays = businessDays;
        }
        public BusinessWeek(string instanceName, TimeSpan startTime, TimeSpan endTime, params DayOfWeek[] dayOfWeeks) : this(instanceName)
        {
            if (dayOfWeeks != null)
                for (int i = 0; i < dayOfWeeks.Length; i++)
                    BusinessDays.Add(new BusinessDay(dayOfWeeks[i], startTime, endTime));
        }

        /// <summary>
        /// 默认实例
        /// </summary>
        public static BusinessWeek Default => Nested.Current;

        /// <summary>
        /// 工作单元列表
        /// </summary>
        public IList<BusinessDay> BusinessDays { get; private set; }

        /// <summary>
        /// 实例名称
        /// </summary>
        public string InstanceName { get; private set; } = (nameof(BusinessWeek) + "_" + DateTime.Now.Ticks);

        /// <summary>
        /// 添加工作单元
        /// </summary>
        /// <param name="businessDay"></param>
        public void AddBusinessDay(BusinessDay businessDay)
        {
            if (InstanceName == nameof(Default))
                throw new InvalidOperationException($"Operation is forbidden as the default instance.");
            if (businessDay != null)
                BusinessDays.Add(businessDay);
        }

        /// <summary>
        /// 批量添加工作单元
        /// </summary>
        /// <param name="businessDays"></param>
        public void AddBusinessDays(IList<BusinessDay> businessDays)
        {
            if (InstanceName == nameof(Default))
                throw new InvalidOperationException($"Operation is forbidden as the default instance.");
            if (businessDays != null)
                foreach (var item in businessDays)
                    BusinessDays.Add(item);
        }

        /// <summary>
        /// 清空工作单元
        /// </summary>
        public void ClearBusinessDays()
        {
            if (InstanceName == nameof(Default))
                throw new InvalidOperationException($"Operation is forbidden as the default instance.");
            if (BusinessDays != null)
                BusinessDays.Clear();
        }

        /// <summary>
        /// 或取两个指定时间范围内的总工时
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public TimeSpan GetBusinessTime(DateTime startDate, DateTime endDate)
        {
            Validate(true);
            var businessTime = TimeSpan.Zero;
            var workingDate = startDate;
            while (workingDate < endDate)
            {
                if (!NextBusinessDay(workingDate, out var businessStart, out var businessDay))
                    break;
                if (businessStart > endDate)
                    break;
                if (businessDay == null)
                    break;

                var timeToEndOfDay = businessDay.EndTime.Subtract(businessStart.TimeOfDay);
                var businessEnd = businessStart.SafeAdd(timeToEndOfDay);

                if (endDate < businessEnd)
                {
                    timeToEndOfDay = endDate.TimeOfDay.Subtract(businessStart.TimeOfDay);
                    businessTime = businessTime.Add(timeToEndOfDay);
                    return businessTime;
                }

                businessTime = businessTime.Add(timeToEndOfDay);
                workingDate = businessEnd;
            }
            return businessTime;
        }

        /// <summary>
        /// 或取两个指定时间范围内的总工时
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public TimeSpan GetBusinessTime(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return GetBusinessTime(startDate.DateTime, endDate.DateTime);
        }

        /// <summary>
        /// 判断指定时间点是否处于工作时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool IsBusinessDay(DateTime date)
        {
            return BusinessDays.Any(day => day.IsBusinessDay(date));
        }

        /// <summary>
        /// 判断指定时间点是否处于工作时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool IsBusinessDay(DateTimeOffset date)
        {
            return IsBusinessDay(date.DateTime);
        }

        /// <summary>
        /// 移除周中指定天的工作单元
        /// </summary>
        /// <param name="dayOfWeek"></param>
        public void RemoveBusinessDays(DayOfWeek dayOfWeek)
        {
            if (InstanceName == nameof(Default))
                throw new InvalidOperationException($"Operation is forbidden as the default instance.");
            if (BusinessDays != null)
            {
                var toRemove = BusinessDays.Where(o => o.DayOfWeek == dayOfWeek);
                foreach (var item in toRemove)
                    BusinessDays.Remove(item);
            }
        }

        /// <summary>
        /// 判断指定时刻开始是否存在下一个工作区间，并通过out返回开始时刻和所在的单个工作单元
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="nextDate"></param>
        /// <param name="businessDay"></param>
        /// <returns></returns>
        internal bool NextBusinessDay(DateTime startDate, out DateTime nextDate, out BusinessDay businessDay)
        {
            nextDate = startDate;
            businessDay = null;
            var tree = GetDayTree();
            for (int i = 0; i < 7; i++)
            {
                var dayOfWeek = nextDate.DayOfWeek;
                if (!tree.ContainsKey(dayOfWeek))
                {
                    nextDate = nextDate.AddDays(1).Date;
                    continue;
                }
                var businessDays = tree[dayOfWeek];
                if (businessDays == null)
                    continue;
                foreach (var day in businessDays)
                {
                    if (day == null)
                        continue;
                    var timeOfDay = nextDate.TimeOfDay;

                    if (timeOfDay >= day.StartTime && timeOfDay < day.EndTime)
                    {
                        businessDay = day;
                        return true;
                    }
                    if (timeOfDay >= day.StartTime)
                        continue;
                    businessDay = day;
                    nextDate = nextDate.Date.SafeAdd(day.StartTime);
                    return true;
                }
                nextDate = nextDate.AddDays(1).Date;
            }
            return false;
        }

        /// <summary>
        /// 判断指定时刻开始是否存在下一个工作单元，并通过out返回开始时刻和所在的单个工作单元
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="nextDate"></param>
        /// <param name="businessDay"></param>
        /// <returns></returns>
        internal bool NextBusinessDay(DateTimeOffset startDate, out DateTimeOffset nextDate, out BusinessDay businessDay)
        {
            var result = NextBusinessDay(startDate.DateTime, out DateTime nDate, out businessDay);
            nextDate = new DateTimeOffset(nDate);
            return result;
        }

        /// <summary>
        /// 验证当前实例是否存在工作单元
        /// </summary>
        /// <param name="throwException"></param>
        /// <returns></returns>
        /// 
        protected virtual bool Validate(bool throwException)
        {
            if (BusinessDays.Any())
                return true;
            if (throwException)
                throw new InvalidOperationException($"The {nameof(BusinessDays)} property must have at least one {nameof(BusinessDay)}.");
            return false;
        }

        private Dictionary<DayOfWeek, IList<BusinessDay>> GetDayTree()
        {
            if (_dayTree != null)
                return _dayTree;
            _dayTree = new Dictionary<DayOfWeek, IList<BusinessDay>>();
            var days = BusinessDays.OrderBy(o => o.DayOfWeek).ThenBy(o => o.StartTime).ToList();
            foreach (var day in days)
            {
                if (!_dayTree.ContainsKey(day.DayOfWeek))
                    _dayTree.Add(day.DayOfWeek, new List<BusinessDay>());
                _dayTree[day.DayOfWeek].Add(day);
            }
            return _dayTree;
        }


        private class Nested
        {
            internal static readonly BusinessWeek Current;

            static Nested()
            {
                Current = new BusinessWeek();
                Current.InstanceName = nameof(Default);
                Current.BusinessDays.Add(new BusinessDay(DayOfWeek.Monday));
                Current.BusinessDays.Add(new BusinessDay(DayOfWeek.Tuesday));
                Current.BusinessDays.Add(new BusinessDay(DayOfWeek.Wednesday));
                Current.BusinessDays.Add(new BusinessDay(DayOfWeek.Tuesday));
                Current.BusinessDays.Add(new BusinessDay(DayOfWeek.Friday));
            }
        }
    }
}
