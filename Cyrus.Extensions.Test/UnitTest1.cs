using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cyrus.Extensions.DateTimeExtensions;

namespace Cyrus.Extensions.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var d = DateTime.Now.Kind;
            var dd = new DateTime(2018, 04, 21, 17, 12, 00).Kind;
            var ddd = new DateTime(2018, 04, 21, 17, 12, 00).ToLocalTime().Kind;
            var dddd = new DateTime(2018, 04, 21, 17, 12, 00).ToUniversalTime().Kind;
            var ddddd = new DateTime(2018, 04, 21, 17, 12, 00).GetDateTimeFormats();
            var dddddd = new DateTime(2018, 04, 21, 17, 12, 00).ToUniversalTime().ToLocalTime();
            var ddddddd = new DateTime(2018, 04, 21, 17, 12, 00).ToLocalTime().ToLocalTime();
            var dddddddd = new DateTime(2018, 04, 21, 17, 12, 00).ToUniversalTime().ToLocalTime().ToUniversalTime();
            var ddddddddd = new DateTime(2018, 04, 21, 17, 12, 00).ToUniversalTime().ToUniversalTime();
            var o = (DateTimeOffset)new DateTime(2018, 04, 21, 17, 12, 00);
            var oo = new DateTimeOffset(new DateTime(2018, 04, 21, 17, 12, 00));
            var ooo = new DateTimeOffset(new DateTime(2018, 04, 21, 17, 12, 00, DateTimeKind.Utc));
            var oooo = new DateTimeOffset(new DateTime(2018, 04, 21, 17, 12, 00, DateTimeKind.Local));
            var ooooo = new DateTimeOffset(new DateTime(2018, 04, 21, 17, 12, 00, DateTimeKind.Unspecified));
            var i = DateTimeOffset.Now.DateTime;
            var ii = DateTimeOffset.Now.UtcDateTime;
            var iii = DateTimeOffset.Now.LocalDateTime;
            var iiii = new DateTimeOffset(new DateTime(2018, 04, 21, 17, 12, 00), TimeSpan.FromHours(7));
            var iiiii = DateTimeOffset.UtcNow;
            var iiiiii = DateTimeOffset.UtcNow.DateTime;
            var iiiiiii = DateTimeOffset.UtcNow.LocalDateTime;
            var iiiiiiii = DateTimeOffset.UtcNow.UtcDateTime;
            var u = iiii.UtcDateTime;
            var uu = iiii.LocalDateTime;
            var uuu = iiii.DateTime;
            var c = DateTimeOffset.UtcNow.ToDateTime();
            var cc = DateTimeOffset.Now.ToDateTime();
            var ccc = iiii.ToDateTime();

            var now = DateTime.Now;
            var e = now.ToEpoch();
            var ee = now.ToUnixTimeSeconds();
            var eee = now.ToEpoch(10000000);
            var eeee = now.ToEpochOffset(10000000);
            var eeeee = e.ToDateTime();

            var f = now.Floor(TimeSpan.FromDays(7));
            var fc = now.Ceiling(TimeSpan.FromDays(7));

            var oNow = new DateTimeOffset(DateTime.SpecifyKind(now, DateTimeKind.Local));
            var of = oNow.Floor(TimeSpan.FromDays(7));
            var ofc = oNow.Ceiling(TimeSpan.FromDays(7));

            var l = oNow.ToEpoch();
            var ll = oNow.ToUnixTimeSeconds();
            var lll = oNow.ToUniversalTime().ToUnixTimeMilliseconds();

            var so = lll.ToDateTimeOffset(TimeSpan.FromHours(8));
            var soo = lll.ToDateTimeOffset(TimeSpan.FromHours(0));
            var sooo = lll.ToDateTimeOffset(TimeSpan.FromHours(14));
            var soooo = lll.ToDateTimeOffset(TimeSpan.FromHours(7.5));
            var sooooo = lll.ToDateTimeOffset();

            var ts = TimeZoneInfo.Local.GetUtcOffset(DateTime.MinValue);
            var tss = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            var tsss = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);

            var to = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.MinValue);
            var too = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.Now);
            var tooo = TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.UtcNow);

            var ul = DateTime.Now - DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            var s0 = new AgeSpan();
            s0.TimeSpan = new TimeSpan(876577376646);
            var s = now.ToAgeString(now.AddYears(-5).AddMonths(4).AddDays(3).AddHours(2).AddMinutes(1).AddSeconds(9).AddMilliseconds(8).AddWeeks(7).AddTicks(6), 100 * 12 * 30 * 24, true);
            var ss = s0.ToString();

            var ey = now.EndOfYear();
            var em = now.EndOfMonth();

            var en = (TEnum)Enum.Parse(typeof(TEnum), "9");

        }
    }

    [Flags]
    public enum TEnum
    {
        One = 1,
        Two = 2,
        Four = 4,
        Eight = 8,
        Sixteen = 16
    }
}
