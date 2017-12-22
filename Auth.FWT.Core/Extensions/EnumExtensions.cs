using System;
using System.ComponentModel;
using System.Reflection;

namespace Auth.FWT.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        public static Tuple<DateTime, DateTime> GetTimeRangeByType(this Enums.Enum.DateRange timeRangeType, int month, int year, DateTime customStart, DateTime customEnd)
        {
            DateTime start = DateTime.Today;
            DateTime end = DateTime.Today;

            switch (timeRangeType)
            {
                case (Auth.FWT.Core.Enums.Enum.DateRange.Last30Days):
                    {
                        end = start.AddDays(1);
                        start = start.AddDays(-30);
                        break;
                    }

                case (Auth.FWT.Core.Enums.Enum.DateRange.Last7Days):
                    {
                        end = start.AddDays(1);
                        start = start.AddDays(-7);
                        break;
                    }

                case (Auth.FWT.Core.Enums.Enum.DateRange.LastMonth):
                    {
                        start = new DateTime(start.Year, start.Month - 1, 1);
                        end = new DateTime(start.Year, start.Month, 1);
                        break;
                    }

                case (Auth.FWT.Core.Enums.Enum.DateRange.ThisMonth):
                    {
                        end = start.AddDays(1);
                        start = new DateTime(start.Year, start.Month, 1);
                        break;
                    }

                case (Auth.FWT.Core.Enums.Enum.DateRange.Today):
                    {
                        end = start.AddDays(1);
                        break;
                    }

                case (Auth.FWT.Core.Enums.Enum.DateRange.Yesterday):
                    {
                        end = start;
                        start = start.AddDays(-1);
                        break;
                    }

                case (Auth.FWT.Core.Enums.Enum.DateRange.LastWeek):
                    {
                        start = start.AddDays(-(int)start.DayOfWeek - 6);
                        end = start.AddDays(7);
                        break;
                    }

                case (Auth.FWT.Core.Enums.Enum.DateRange.ThisWeek):
                    {
                        start = start.StartOfWeek();
                        end = start.AddDays(7);
                        break;
                    }

                case (Auth.FWT.Core.Enums.Enum.DateRange.MonthYear):
                    {
                        start = new DateTime(year, month, 1);
                        end = start.AddMonths(1);
                        break;
                    }

                case (Auth.FWT.Core.Enums.Enum.DateRange.Custom):
                    {
                        return new Tuple<DateTime, DateTime>(customStart, customEnd.AddDays(1));
                    }
            }

            return new Tuple<DateTime, DateTime>(start, end);
        }
    }
}
