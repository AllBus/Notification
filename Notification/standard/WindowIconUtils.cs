using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Notification.standard
{
    public static class WindowIconUtils
    {
        public static void updateIcon(System.Windows.Window window, DataStore dataStore, ref long iconTime)
        {
            long start = Utils.dateTimeToLong(DateTime.UtcNow.AddHours(-1));
            long end = Utils.dateTimeToLong(DateTime.UtcNow.AddDays(1));
            //var m = dataStore.items.Max(x => x.date);
            //Console.Out.WriteLine(m + " - " + start + " = " +
            //    (m - start)
            //    +" "+ Utils.longToDateTime(m).ToString(Constants.longDateFormat)
            //    + " " + Utils.longToDateTime(start).ToString(Constants.longDateFormat));
            var ev = dataStore.items.Find(x => x.date >= start && x.date <= end);
            if (ev != null)
            {
                if (iconTime != ev.date)
                {
                    iconTime = ev.date;
                    var date = Utils.longToDateTime(ev.date);
                    window.Icon = createTimeIcon(date, 0f,ev.kindColor);
                }

            }
            else
            {
                if (iconTime != 0)
                {
                    iconTime = 0;
                    window.Icon = Utils.bitmapToSource(Properties.Resources.notifiicationPro.ToBitmap());
                }
            }

        }
        
        public static BitmapImage createTimeIcon(DateTime time, float percent,int color)
        {
            int pt = 3;
            Font font = new Font("Tahoma", 6*pt);
            Font minuteFont = new Font("Tahoma", 4*pt);
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.White);
            System.Drawing.Pen bottomLine = new System.Drawing.Pen(System.Drawing.Color.FromArgb(color));
            //   pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Right;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            sf.LineAlignment = StringAlignment.Center;


            var bit = new Bitmap(64, 64);//*/(24 * pt, 24 * pt);
            var canvas = Graphics.FromImage(bit);
            //        canvas.Clear(Color.Yellow);

            canvas.DrawString(time.Hour.ToString(), font, pen.Brush, 16*pt, 12*pt, sf);

            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;

            canvas.DrawString(time.ToString("mm"), minuteFont, pen.Brush, 15*pt, 12*pt, sf);

            canvas.FillRectangle(bottomLine.Brush, 0 + 24*pt * percent, 18*pt, 24 * pt * (1f - percent), 6*pt);

            return Utils.bitmapToSource(bit);

        }
    }
}
