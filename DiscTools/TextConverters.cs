using System;
using System.Collections.Generic;
using System.Text;

namespace DiscTools.Inspection
{
    public class TextConverters
    {
        public static string TruncateLongString(string str, int maxLength)
        {
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public static DateTime? ParseDiscDateTime(string dtString)
        {
            if (dtString.Contains("0000000"))
                return null;
            try
            {
                DateTime dt = DateTime.ParseExact(dtString, "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture);
                return dt;
            }
            catch
            {
                return null;
            }
        }
    }
}
