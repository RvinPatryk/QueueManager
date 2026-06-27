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

namespace QueueManager
{
    public partial class MainWindow : Window
    {
        public MainWindow(User loggedUser)
        {
            InitializeComponent();
            DataContext = new MainViewModel(loggedUser);
        }

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
    }
}