using System;
using System.Windows;
using QueueManager.Services;
using QueueManager;


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

                var mainWindow = new MainWindow();
                mainWindow.Show();

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