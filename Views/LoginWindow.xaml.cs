using System;
using System.Windows;
using QueueManager.Services;
using QueueManager;
using QueueManager.Models;


namespace QueueManager.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService = new();

        public LoginWindow()
        {
            InitializeComponent();

            Loaded += (_, _) => UsernameTextBox.Focus();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = UsernameTextBox.Text.Trim();
                string password = PasswordBox.Password;

                var user = _authService.Login(username, password);

                if (user == null)
                {
                    ErrorTextBlock.Text = "Nieprawidłowy login lub hasło.";
                    PasswordBox.Clear();
                    PasswordBox.Focus();

                    AppLogger.Warning(
                        $"Nieudana próba logowania dla użytkownika: '{username}'.");

                    return;
                }

                AppLogger.Info(
                    $"Zalogowano użytkownika: '{user.Username}', rola: {user.Role}.");

                if (user.Role == UserRole.Admin)
                {
                    var mainWindow = new MainWindow(user);
                    mainWindow.Show();
                }
                else
                {
                    var userWindow = new UserWindow(user);
                    userWindow.Show();
                }

                Close();

                Close();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas logowania użytkownika.", ex);

                ErrorTextBlock.Text =
                    $"Wystąpił błąd podczas logowania: {ex.Message}";
            }
        }
    }
}