using QueueManager.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using QueueManager.Models;
using QueueManager.Views;

namespace QueueManager
{
    public partial class MainWindow : Window
    {

        public MainWindow(User loggedUser)
        {
            InitializeComponent();

            _loggedUser = loggedUser;
            DataContext = new MainViewModel(loggedUser);
        }

        private readonly User _loggedUser;

        private void TasksDataGrid_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (DataContext is MainViewModel vm &&
                sender is DataGrid dataGrid)
            {
                vm.SelectedTasks = dataGrid.SelectedItems;
            }
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();

            Close();
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            var usersWindow = new UserManagementWindow(_loggedUser)
            {
                Owner = this
            };

            usersWindow.ShowDialog();

            if (DataContext is MainViewModel viewModel)
            {
                viewModel.LoadUsernames();
            }
        }
    }
}