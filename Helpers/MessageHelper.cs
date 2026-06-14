using System.Windows;

namespace QueueManager.Helpers
{
    public static class MessageHelper
    {
        public static void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "B³¹d",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public static void ShowInfo(string message)
        {
            MessageBox.Show(
                message,
                "Informacja",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public static bool Confirm(string message)
        {
            var result = MessageBox.Show(
                message,
                "Potwierdzenie",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            return result == MessageBoxResult.Yes;
        }
    }
}