using GPLX.Core.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GPLX.Infrastructure.Extensions
{
    public static class DateTimeParse
	{
		public static DateTime ParseDefault(this string input, string[] format = null)
		{
			var formats = format ?? new string[]
			{
				"yyyy/MM/dd",
				"yyyy/MM/dd HH:mm:ss",
				"yyyy-MM-dd",
				"yyyy-MM-dd HH:mm:ss",
				"yyyyMMdd",
				"yyyyMMdd HH:mm:ss",
				"yyyy-MM-ddTHH:mm:ss.fffZ"
			};

			foreach (var item in formats)
			{
				if (DateTime.TryParseExact(input, item, null, DateTimeStyles.None, out DateTime dateTime))
				{
					return dateTime;
				}
			}
			throw new FormatException("Can not convert datetime");
		}

		public static DateTime FirstDateOfWeekISO8601(this int year, int weekOfYear)
        {
            var wiy = WeekInYear(year);
            return wiy.First(c => c.weekNum == weekOfYear).weekStart;
        }

		public static IList<WeekInYear> WeekInYear(this int year)
        {
			var jan1 = new DateTime(year, 1, 1);
			//beware different cultures, see other answers
			var startOfFirstWeek = jan1.AddDays(1 - (int)(jan1.DayOfWeek));
			var weeks =
				Enumerable
					.Range(0, 54)
					.Select(i => new {
						weekStart = startOfFirstWeek.AddDays(i * 7)
					})
					.TakeWhile(x => x.weekStart.Year <= jan1.Year)
					.Select(x => new {
						x.weekStart,
						weekFinish = x.weekStart.AddDays(6)
					})
					.SkipWhile(x => x.weekFinish < jan1.AddDays(1))
					.Select((x, i) => new WeekInYear{
						weekStart = x.weekStart,
						weekFinish = x.weekFinish,
						weekNum = i + 1
					}).ToList();

			return weeks;
		}

		public static int GetIso8601WeekOfYear(this DateTime time)
		{
			// Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll
			// be the same week# as whatever Thursday, Friday or Saturday are,
			// and we always get those right
			DayOfWeek day = CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(time);
			if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
			{
				time = time.AddDays(3);
			}

			// Return the week of our adjusted day
			return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
		}
	}
}
