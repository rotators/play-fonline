namespace PlayFOnline
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public static class Utils
    {
        public static int GetCurrentUnixTime()
        {
            int unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            return unixTimestamp;
        }

        public static string GetFilenameFromUrl(string url)
        {
            Uri uri = new Uri(url);
            return uri.Segments[uri.Segments.Length - 1];
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

        public static DateTime GetUnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return time.AddSeconds(unixTimeStamp).ToLocalTime();
        }
    }
}