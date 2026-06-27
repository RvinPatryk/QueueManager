using System.Windows;
using Microsoft.EntityFrameworkCore;
using QueueManager.Data;
using QueueManager.Services;

namespace QueueManager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            using (var db = new QueueManagerDbContext())
            {
                db.Database.Migrate();
            }

            var authService = new AuthService();
            authService.CreateDefaultAdmin();

            base.OnStartup(e);
        }
    }
}