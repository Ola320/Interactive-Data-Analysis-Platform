using System.Windows;
using DataAnalizer.Data;

namespace DataAnalizer
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using var database = new AppDbContext();
            database.Database.EnsureCreated();
        }
    }
}