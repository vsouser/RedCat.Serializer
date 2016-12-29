using System;

namespace RedCat.Serializer.Utils
{
    public static class UnixTime
    {
        private static DateTime UnixEpoch
        {
            get { return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); }
        }

        public static string Now
        {
            get
            {
                return ((long)(DateTime.UtcNow.Subtract(UnixEpoch)).TotalSeconds).ToString();
            }
        }

    }
}
