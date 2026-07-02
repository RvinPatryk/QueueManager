using System.Windows;
using System.Windows.Controls;
using QueueManager.Models;
using QueueManager.ViewModels;

namespace QueueManager.Views
{
    public partial class UserManagementWindow : Window
    {
        public UserManagementWindow(User loggedAdmin)
        {
            InitializeComponent();
            DataContext = new UserManagementViewModel(loggedAdmin);
        }

        private void NewPasswordBox_PasswordChanged(
            object sender,
            RoutedEventArgs e)
        {
            if (DataContext is UserManagementViewModel viewModel)
            {
                viewModel.NewPassword = NewPasswordBox.Password;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            NewPasswordBox.Clear();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void UsersDataGrid_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (DataContext is UserManagementViewModel viewModel &&
                viewModel.SelectedUser == null)
            {
                NewPasswordBox.Clear();
            }
        }
    }
}