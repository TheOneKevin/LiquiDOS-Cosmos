using Cosmos.HAL;

namespace LiquiDOS.Drivers
{
    public static class Time
    {
        public static int Hour() { return RTC.Hour; }

        public static int Minute() { return RTC.Minute; }

        public static int Second() { return RTC.Second; }

        public static int Century() { return RTC.Century; }

        public static int Year() { return RTC.Year; }

        public static int Month() { return RTC.Month; }

        public static int DayOfMonth() { return RTC.DayOfTheMonth; }

        public static int DayOfWeek() { return RTC.DayOfTheWeek; }
    }
}
