using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace PlayFO
{
    public static class Utils
    {
        public static string GetFilenameFromUrl(string Url)
        {
            Uri uri = new Uri(Url);
            return uri.Segments[uri.Segments.Length - 1];
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static Int32 GetCurrentUnixTime()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        public static string GetReadableTime(int seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            if (t.Days > 0) return t.Days + " days";
            if (t.Hours > 0) return t.Hours + " hours";
            if (t.Minutes > 0) return t.Minutes + " minutes";
            return seconds + " seconds";
        }

        public static string GetSHA1Checksum(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            using (BufferedStream bs = new BufferedStream(fs))
            {
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    byte[] hash = sha1.ComputeHash(bs);
                    StringBuilder formatted = new StringBuilder(2 * hash.Length);
                    foreach (byte b in hash)
                    {
                        formatted.AppendFormat("{0:X2}", b);
                    }
                    return formatted.ToString();
                }
            }
        }
    }
}
