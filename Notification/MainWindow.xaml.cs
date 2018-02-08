using System;
using System.Collections.Generic;

using System.Windows.Shell;
using System.Windows.Controls;

using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

using System.Windows.Threading;
using Notification.standard;
using System.Linq;

namespace Notification
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : System.Windows.Window
    {
        private const String APP_ID = "Главные события";
        private long iconTime = 0;
        DataStore dataStore= new DataStore();
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            ProgramProperty.load();

            

            String windowTitle = ProgramProperty.windwowTitle;
            if (windowTitle.Length > 0)
            {
                this.Title = windowTitle;
            }


            buttonCreate.Visibility = (ProgramProperty.buttonEditVisible) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            int seconds = (int)ProgramProperty.period;
            if (seconds < 15)
                seconds = 15;

            timer.Interval = new TimeSpan(0, 0,seconds);
            timer.Tick += new EventHandler(dispatcherTimer_Tick);
            timer.Start();

            dataStore.lastDate=Properties.Settings.Default.LastUpdateTime;
            updateItems(true);
            //TaskbarItemInfo

            ////
            //TabbedThumbnail preview = new TabbedThumbnail(this.Handle, tabPage.Handle);
            ////
            //preview.TabbedThumbnailActivated += new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailActivated);
            //preview.TabbedThumbnailClosed += new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailClosed);
            //preview.TabbedThumbnailMaximized += new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailMaximized);
            //preview.TabbedThumbnailMinimized += new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailMinimized);
            ////
            //TaskbarManager.Instance.TabbedThumbnail.AddThumbnailPreview(preview);
            ////
            //tabControl1.SelectedTab = tabPage;
            //TaskbarManager.Instance.TabbedThumbnail.SetActiveTab(tabControl1.SelectedTab);
            ////
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            updateItems(false);
        }



        private async void updateItems(bool needLoad)
        {
            var newStore = await dataStore.updateListAsync(needLoad);
       //     Console.Out.WriteLine("update items");
            if (newStore != dataStore)
            {
                List<EventData> notifityItems = dataStore.newItems(Utils.Now, newStore);
                dataStore = newStore;
          //      Console.Out.WriteLine("new items "+notifityItems.Count);
                refreshData();

                if (Properties.Settings.Default.LastUpdateTime != dataStore.lastDate)
                {
                    Properties.Settings.Default.LastUpdateTime=dataStore.lastDate;
                    Properties.Settings.Default.Save();
                    notifityData(notifityItems);
                }
            }
            else
            {
                refreshIcon();
            }

        }

        private void sendBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
                    
        }

        private void notifityData(List<EventData> datas)
        {
            if (datas.Count > 0)
            {
                var data = datas.Last();

                if (Utils.isWin10())
                {
                    notifityCenterToast(data);
                }

                try
                {
                    taskBarItemInfo1.Description = data.shortInfo;
                    taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Paused;
                    taskBarItemInfo1.ProgressValue = 100;
                }
                catch { }

                try
                {
                    var contentPage = fullDataFrame.Content as previewEventPage;

                    contentPage.bind(data);

                }
                catch { }

                try
                {
                    SuperBarNameSpace.NativeMethods.FlashWindow(this, 3);
                }
                catch { }
            }
        }

        private void notifityCenterToast(EventData data)
        {
            try
            {
                // Get a toast XML template
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);

                // Fill in the text elements
                XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                for (int i = 0; i < stringElements.Length; i++)
                {
                    stringElements[i].AppendChild(toastXml.CreateTextNode(data.info));
                }

                // Specify the absolute path to an image
                //string imagePath = "file:///" + Path.GetFullPath("toastImageAndText.png");
                //XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                //imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

                // Create the toast and attach event listeners
                ToastNotification toast = new ToastNotification(toastXml);

                toast.ExpirationTime = new DateTimeOffset(DateTime.Now.AddDays(1));
                toast.Activated += ToastActivated;
                toast.Dismissed += ToastDismissed;
                toast.Failed += ToastFailed;

                // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
                ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
            }
            catch
            {

            }
        }

        private void refreshIcon()
        {
            try
            {
                WindowIconUtils.updateIcon(this, dataStore, ref iconTime);
            }
            catch { }
        }

        private void refreshData()
        {
            refreshIcon();

            try
            {
                //  Bitmap myImage = new Bitmap(Notification.Properties.Resources.stopImg);
                //    taskBarItemInfo1.Overlay = (DrawingImage)Notification.Properties.Resources.playImg;
                calendarList.Items.Clear();
                foreach (EventData d in dataStore.items)
                {
                    var cv = new CalendarPage();
                    cv.Width = 200;
                    cv.Height = 250;
                    cv.bind(d);

                    var fr = new Frame();
                    fr.Content = cv;
                    var li = new ListBoxItem();

                    li.Content = fr;
                    calendarList.Items.Add(li);
                }
            }
            catch
            {

            }
        }
        

        private void ToastActivated(ToastNotification sender, object e)
        {
            Dispatcher.Invoke(() =>
            {
                taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
                //TODO: start process
              //  Process.Start(editLink.Text);

                // Activate();
                //  Title = "The user activated the toast.";
            });
        }

        private void ToastDismissed(ToastNotification sender, ToastDismissedEventArgs e)
        {
            String outputText = "";
            switch (e.Reason)
            {
                case ToastDismissalReason.ApplicationHidden:
                    outputText = "The app hid the toast using ToastNotifier.Hide";
                    break;
                case ToastDismissalReason.UserCanceled:
                    outputText = "The user dismissed the toast";
                    break;
                case ToastDismissalReason.TimedOut:
                    outputText = "The toast has timed out";
                    break;
            }

            Dispatcher.Invoke(() =>
            {
                //
                Title = outputText;
            });
        }

        private void ToastFailed(ToastNotification sender, ToastFailedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                //
                Title = "The toast encountered an error.";
            });
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
            SuperBarNameSpace.NativeMethods.StopFlashingWindow(this);
        }

        private void calendarList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = calendarList.SelectedItem;
            if (item is ListBoxItem)
            {
                EventData data = (((item as ListBoxItem).Content as Frame).Content as CalendarPage).data;
                var contentPage = fullDataFrame.Content as previewEventPage;

                contentPage.bind(data);

            }
        }

        private void button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            updateItems(true);
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private long closingTime = DateTime.Now.Ticks;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var now = DateTime.Now;
            if (now.Ticks > closingTime)
            {
                closingTime = now.AddSeconds(2).Ticks;
                e.Cancel = true;
                this.WindowState = System.Windows.WindowState.Minimized;
            }
            
        }

        private void buttonCreate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EventEditorWindow eventWindow = new EventEditorWindow();
            eventWindow.Owner = this;
            if (eventWindow.ShowDialog() == true)
            {
                updateItems(true);
            }
        }

        private void button_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void calendarList_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta < 0)
            {
                if (calendarList.SelectedIndex < calendarList.Items.Count - 1)
                    calendarList.SelectedIndex++;
            }
            else {
                if (calendarList.SelectedIndex > 0)
                    calendarList.SelectedIndex--;
            }
            var obj = calendarList.SelectedItem;
            if (obj!=null)
                calendarList.ScrollIntoView(obj);
            e.Handled = true;
        }
    }
}
