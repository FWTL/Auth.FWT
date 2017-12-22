using System.ComponentModel;

namespace Auth.FWT.Core.Enums
{
    public static class Enum
    {
        public enum ApplicationType
        {
            JavaScript = 0,
            NativeConfidential = 1
        }

        public enum UserRole
        {
            Admin = 1,
            User = 2,
        }

        public enum DateRange
        {
            [Description("All")]
            All = 1,

            [Description("Today")]
            Today = 2,

            [Description("Yesterday")]
            Yesterday = 3,

            [Description("This Week")]
            ThisWeek = 4,

            [Description("Last Week")]
            LastWeek = 5,

            [Description("Last 7 days")]
            Last7Days = 6,

            [Description("This Month")]
            ThisMonth = 7,

            [Description("Last Month")]
            LastMonth = 8,

            [Description("Last 30 days")]
            Last30Days = 9,

            [Description("Given Month")]
            MonthYear = 10,

            [Description("Custom")]
            Custom = 11,
        }
    }
}