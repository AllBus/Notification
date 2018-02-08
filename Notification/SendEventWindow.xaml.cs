using System;

using System.Windows;


namespace Notification
{
    /// <summary>
    /// Логика взаимодействия для SendEventWindow.xaml
    /// </summary>
    public partial class SendEventWindow : Window
    {
        public EventData data = EventData.none;
        public SendEventWindow(EventData data)
        {
            this.data = data;
            InitializeComponent();

            authorLayout.Visibility= (ProgramProperty.buttonAuthorVisible) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            authorEdit.Text = Properties.Settings.Default.authorName;
            // bind(data);
        }

        
        private async void sendBtn_Click(object sender, RoutedEventArgs e)
        {
            sendBtn.IsEnabled = false;
            int res = 0;
            if (ProgramProperty.buttonAuthorVisible)
            {
                if (authorEdit.Text.Length < 3)
                {
                    MessageBox.Show("Введите имя автора длиной не менее 3 символов");
                    authorEdit.Focus();
                    sendBtn.IsEnabled = true;
                    return;
                }
                if (passwordEdit.Password.Length < 6)
                {
                    MessageBox.Show("Пароль должен быть не короче 6 символов");
                    passwordEdit.Focus();
                    sendBtn.IsEnabled = true;
                    return;
                }
                Properties.Settings.Default.authorName = authorEdit.Text;
                Properties.Settings.Default.Save();

                res=await DataStore.uploadEvent(data,true,authorEdit.Text,passwordEdit.Password);
            }
            else {
                res=await DataStore.uploadEvent(data);
            }

            if (res == 0)
            {
                DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка при отправке сообщения");
                sendBtn.IsEnabled = true;
            }
        }

        public void bind(EventData data)
        {
            this.data = data;

            var contentPage = fullDataFrame.Content as previewEventPage;
            if (contentPage != null)
            {
                contentPage.bind(data);
            }

            var calendarPage = calendarDataFrame.Content as CalendarPage;
            if (calendarPage != null)
            {
                calendarPage.bind(data);
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
          
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void calendarDataFrame_Initialized(object sender, EventArgs e)
        {
          //  bind(data);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            bind(data);
        }

        private void fullDataFrame_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void calendarDataFrame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            bind(data);
        }

        private void fullDataFrame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            bind(data);
        }
    }
}
