using System.Windows;
using QueueManager.Models;
using QueueManager.ViewModels;

namespace QueueManager.Views
{
    public partial class UserWindow : Window
    {
        public UserWindow(User loggedUser)
        {
            InitializeComponent();
            DataContext = new UserViewModel(loggedUser);
        }
    }
}