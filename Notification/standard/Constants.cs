using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification
{
    class Constants
    {
       public const string dateLabelFormat = "d MMMM";
       public const string dateYearLabelFormat = "d MMMM yy";
       public const string timeLabelFormat = "HH:mm";
       public const string longDateFormat = "d MMMM yyyy в HH:mm";

       internal static string API_URI = "http://127.0.0.1/" ;

       internal static string API_CHECK_DATE(long date)
       {
            return "events/checkLastTime/" + date;
       }

       internal const string API_EVENT_LIST = "events/now";

        internal static string API_EVENT(int id)
        {
            return "event/" + id;
        }

        internal static string API_CHECK_LOAD(long date)
        {
            return "events/now/" + date + "/";
        }

        internal static string API_UPLOAD_EVENT = "events/add";
        internal static string API_UPLOAD_EVENT_WITH_IMAGE = "eventWithImage";
    }
}
