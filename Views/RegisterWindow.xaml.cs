using System;
using System.Windows;
using QueueManager.Models;
using QueueManager.Services;

namespace QueueManager.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly AuthService _authService = new();

        public RegisterWindow()
        {
            InitializeComponent();

            Loaded += (_, _) => UsernameTextBox.Focus();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = UsernameTextBox.Text.Trim();
                string password = PasswordBox.Password;
                string repeatPassword = RepeatPasswordBox.Password;

                if (string.IsNullOrWhiteSpace(username))
                {
                    ErrorTextBlock.Text = "Nazwa użytkownika jest wymagana.";
                    UsernameTextBox.Focus();
                    return;
                }

                if (username.Length < 3)
                {
                    ErrorTextBlock.Text = "Nazwa użytkownika musi mieć co najmniej 3 znaki.";
                    UsernameTextBox.Focus();
                    return;
                }

                if (password.Length < 6)
                {
                    ErrorTextBlock.Text = "Hasło musi mieć co najmniej 6 znaków.";
                    PasswordBox.Focus();
                    return;
                }

                if (password != repeatPassword)
                {
                    ErrorTextBlock.Text = "Hasła nie są takie same.";
                    RepeatPasswordBox.Clear();
                    RepeatPasswordBox.Focus();
                    return;
                }

                bool registered = _authService.Register(
                    username,
                    password,
                    UserRole.User);

                if (!registered)
                {
                    ErrorTextBlock.Text =
                        "Nie udało się utworzyć konta. Nazwa użytkownika może być już zajęta.";
                    return;
                }

                AppLogger.Info(
                    $"Zarejestrowano nowego użytkownika: '{username}'.");

                MessageBox.Show(
                    "Konto zostało utworzone. Możesz się teraz zalogować.",
                    "Rejestracja",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Close();
            }
            catch (Exception ex)
            {
                AppLogger.Error("Błąd podczas rejestracji użytkownika.", ex);

                ErrorTextBlock.Text =
                    $"Nie udało się utworzyć konta: {ex.Message}";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}