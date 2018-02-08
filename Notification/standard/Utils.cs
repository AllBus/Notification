using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Notification
{
    class Utils
    {
        private static DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        public static DateTime longToDateTime(long date) => unixEpoch.AddMilliseconds(date).ToLocalTime();
        //TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(date * 10000).Add(unixEpoch));


        public static long dateTimeToLong(DateTime dateTime) => dateTime.Subtract(unixEpoch).Ticks / 10000;

        public static System.Windows.Media.Brush intToColorBrush(int color)
        {
            byte[] bytes = BitConverter.GetBytes(color);
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]));
        }


        private static bool bWin10 = false;
        private static bool bCheckVersion = false;
        internal static int DAY = 24*3600*1000;

        public static bool isWin10()
        {
            if (bCheckVersion)
            {
                return bWin10;
            }
            else
            {
                //TODO: нужно как-то проверить
                bCheckVersion = true;
                bWin10 = false;// Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 2;
                return bWin10;
            }

            //return true; // Environment.OSVersion.Version.Major >= 10;
        }

        internal static String durationToString(long duration)
        {
            long sec = duration / 1000;

            var min = sec / 60;
            var hour = min / 60;
            var days = hour / 24;

            hour = hour % 24;
            min = min % 60;
            sec = sec % 60;
            StringBuilder sb = new StringBuilder();
            if (days > 0)
            {

                sb.Append(days);
                if (days == 1)
                {
                    sb.Append(" день");
                }
                else
                if (days < 5)
                {
                    sb.Append(" дня");
                }
                else
                {
                    sb.Append(" дней");
                }
            }
            else
            {

                if (hour > 0)
                {
                    sb.Append(hour);
                    if (hour == 1)
                    {
                        sb.Append(" час");
                    }
                    else
                      if ((hour - 1) % 10 < 4 && (hour < 10 || hour > 20))
                    {
                        sb.Append(" часа");
                    }
                    else
                    {
                        sb.Append(" часов");
                    }
                    if (min > 0)
                        sb.Append(" ");
                }

                if (min > 0)
                {
                    sb.Append(min);
                    if (min == 1)
                    {
                        sb.Append(" минута");
                    }
                    else
                    if ((min - 1) % 10 < 4 && (min < 10 || min > 20))
                    {
                        sb.Append(" минуты");
                    }
                    else
                    {
                        sb.Append(" минут");
                    }

                }
            }
            return sb.ToString();
        }

        public static BitmapImage bitmapToSource(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        public static long Now { get { return dateTimeToLong(DateTime.UtcNow); } }

        public static void browseFile(string uri)
        {
            try {
                if (uri.StartsWith("http://") || uri.StartsWith("https://"))
                    Process.Start(uri);
            } catch
            {

            }
        }
    }
}
