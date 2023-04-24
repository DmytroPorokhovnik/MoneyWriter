using System.Net.Http;
using System.Windows;
using UI.Services;
using UI.ViewModels;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //should be Dependency injection
            MainWindow window = new MainWindow(new MainWindowViewModel(new MoneyConversionService(new HttpClient())));
            window.Show();
        }
    }
}
