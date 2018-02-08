using System;
using System.Windows.Controls;


namespace Notification
{
    /// <summary>
    /// Логика взаимодействия для CalendarPage.xaml
    /// </summary>
    public partial class CalendarPage : Page
    {
        public EventData data = EventData.none;

        public CalendarPage()
        {
            InitializeComponent();
        }

        internal void bind(EventData data)
        {
            this.data = data;

            var date = Utils.longToDateTime(data.date);
            var nextYear = DateTime.Now.AddMonths(8);
            dateLabel.Content =   
                date.ToString((date > nextYear) ? Constants.dateYearLabelFormat : Constants.dateLabelFormat);
            timeLabel.Content = date.ToString(Constants.timeLabelFormat);
            durationLabel.Content = Utils.durationToString(data.duration);
            infoText.Text = data.shortInfo;
            kindLabel.Content = data.kind;
            var colorBrush=  Utils.intToColorBrush(data.kindColor);
            kindBorder.Background = colorBrush;
            border.BorderBrush = colorBrush;
        }
    }
}
