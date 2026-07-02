using System.Windows;
using System.Windows.Controls;
using QueueManager.Models;
using QueueManager.Services;
using QueueManager.ViewModels;

namespace QueueManager.Views
{
    public partial class UserWindow : Window
    {
        private readonly User _loggedUser;
        public UserWindow(User loggedUser)
        {
            InitializeComponent();

            _loggedUser = loggedUser;
            DataContext = new UserViewModel(loggedUser);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AppLogger.Info($"Użytkownik '{_loggedUser.Username}' wylogował się.");

            var loginWindow = new LoginWindow();
            loginWindow.Show();

            Close();
        }
    }
}