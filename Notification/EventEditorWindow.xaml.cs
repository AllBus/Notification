using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;




namespace Notification
{
    /// <summary>
    /// Логика взаимодействия для EventEditorWindow.xaml
    /// </summary>
    public partial class EventEditorWindow : Window
    {
        public EventData data = new EventData();
        public EventEditorWindow()
        {
            InitializeComponent();
        
          //  updatePreview();
        }
        private void updatePreview()
        {
            var contentPage = fullDataFrame.Content as previewEventPage;
            if (contentPage != null)
            {
                contentPage.bind(data);
            }
        }

        private void timeEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateDataTime();
        }

        private void colorBtn_Click(object sender, EventArgs e)
        {
            var color = colorBtn.Color;
            data.kindColor = BitConverter.ToInt32(new byte[] { color.B, color.G, color.R, color.A }, 0); ;
            updatePreview();
        }

        private void uriEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            data.uri = uriEdit.Text;
            updatePreview();
        }

     
        private void imageEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            data.image = imageEdit.Text;
            updatePreview();
        }

        private void kindEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            data.kind = kindEdit.Text;
            updatePreview();
        }

        private void colorBtn_Closed(object sender, RoutedEventArgs e)
        {
   
        }

        private void nameEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            data.name = nameEdit.Text;
            updatePreview();
        }

        private void shortInfoEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            data.shortInfo = shortInfoEdit.Text;
            updatePreview();
        }

        private void infoEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            data.info = infoEdit.Text;
            updatePreview();
        }

        private void imageBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Изображения(*.JPG;*.PNG)|*.JPG;*.GIF;*.PNG";
            if (dialog.ShowDialog() == true)
            {
                imageEdit.Text = dialog.FileName;
            }
        }

        private void dateEdit_CalendarClosed(object sender, RoutedEventArgs e)
        {
          
        }

        private void updateDataTime()
        {
            long time = extractTime(timeEdit.Text);
            if (time < 0)
                return;

            DateTime? dt = dateEdit.SelectedDate;
            if (dt != null) {
                
                data.date = Utils.dateTimeToLong((dt ?? DateTime.Now).ToUniversalTime()) + time;
                updatePreview();
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        public long extractTime(String text)
        {
            var sp = text.Split(':');
            const long second= 1000;

            if (sp.Length == 2)//hh:mm
            {
                var ht = sp[0].Trim();
                var mt = sp[1].Trim();

                int h = 0;
                int m = 0;

                int.TryParse(ht, out h);
                int.TryParse(mt, out m);

                return (60*h+m)*60 * second;
            }
            else
            {
                return -1;
            }

        }

        private void durationEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            var d = extractTime(durationEdit.Text);
            if (d >= 0)
            {
                data.duration = d;
                updatePreview();
            }
        }

        private void colorBtn_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
                var color = (//colorBtn.SelectedColor ??
                Color.FromRgb(255, 255, 255));
                data.kindColor = BitConverter.ToInt32(new byte[] { color.B, color.G, color.R, color.A }, 0); ;
                updatePreview();
        }

        private void completeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (data.image.Length < 2)
            {
                MessageBox.Show("Укажите URL или файл изображения");
                imageEdit.Focus();
                return;
            }

            if (data.date < 1000 * Utils.DAY)
            {
                MessageBox.Show("Укажите дату");
                dateEdit.Focus();
                return;
            }

            if (data.isLocalImage() && !File.Exists(data.image))
            {
                MessageBox.Show("Указанное изображение не найдено");
                imageEdit.Focus();
                return;
            }

            //completeBtn.IsEnabled = false;
            // DataStore.uploadEvent(data);
            //DialogResult = true;
            //this.Close();

            SendEventWindow dialog = new SendEventWindow(data);
            dialog.Owner = this;
            dialog.bind(data);
            if (dialog.ShowDialog() == true)
            {
                DialogResult = true;
                this.Close();
            }
        }

        private void dateEdit_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            
            updateDataTime();
        }

        private void dateEdit_LostFocus(object sender, RoutedEventArgs e)
        {
          
        }

        private void colorBtn_Click_1(object sender, RoutedEventArgs e)
        {
           
        }

       
    }
}
