using System;

namespace Auth.FWT.Core.Helpers
{
    public static class DateTimeHelper
    {
        private static Random _random;

        static DateTimeHelper()
        {
            _random = new Random();
        }

        public static DateTime Random(DateTime start, DateTime end)
        {
            TimeSpan timeSpan = end - start;
            TimeSpan newSpan = new TimeSpan(0, _random.Next(0, (int)timeSpan.TotalMinutes), 0);
            return start + newSpan;
        }
    }
}
