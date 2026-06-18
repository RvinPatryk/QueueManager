using System.Windows;

namespace QueueManager.Views
{
    public partial class LogWindow : Window
    {
        public LogWindow(string logs)
        {
            InitializeComponent();
            LogsTextBox.Text = logs;
            LogsTextBox.ScrollToEnd();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}