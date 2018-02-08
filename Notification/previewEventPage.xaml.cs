using System;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace Notification
{
    /// <summary>
    /// Логика взаимодействия для previewEventPage.xaml
    /// </summary>
    public partial class previewEventPage : Page
    {
        public EventData data = EventData.none;
        private String lastImageUri = "";

        public previewEventPage()
        {
            InitializeComponent();

            bind(data);
        }

        public void gotoItemWebPage()
        {
            if (data.uri.Length > 0)
            {
                Utils.browseFile(data.uri);
            }
        }

        private void linkBtn_Click(object sender, RoutedEventArgs e)
        {
            gotoItemWebPage();
        }

        private void image_TouchUp(object sender, TouchEventArgs e)
        {
            if (ProgramProperty.buttonImageClickable)
                gotoItemWebPage();
        }
        
        private void image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ProgramProperty.buttonImageClickable)
                gotoItemWebPage();
        }

        internal void bind(EventData data)
        {
            this.data = data;
            //  this.Resources["eventData"]= data;
            


            var date = Utils.longToDateTime(data.date);

            contentLabel.Text = data.info;
            dateTimeLabel.Content = data.date==0 ? "": date.ToString(Constants.longDateFormat);
            kindLabel.Content = data.kind;
            durationLabel.Content = Utils.durationToString(data.duration);
            kindLabelBorder.Background = Utils.intToColorBrush(data.kindColor);
            linkBtn.ToolTip = data.uri;
            linkBtn.Visibility = data.uri.Length > 0 ?Visibility.Visible: Visibility.Hidden ;

            if (lastImageUri != data.image)
            {
                try
                {
                    lastImageUri = data.image;
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(data.image, UriKind.Absolute);
                    bitmap.EndInit();
                    image.Source = bitmap;
                }
                catch
                {
                    Console.Out.WriteLine("Don't load");
                    image.Source = null;
                }
            }

        }
    }
}
