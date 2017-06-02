using Inspector.ViewModel;
using System.Windows;

namespace Inspector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new MainWindow();

            var viewModel = new MainWindowViewModel();
            //viewModel.RequestClose += delegate { window.Close(); };

            window.DataContext = viewModel;
            window.Show();
        }
    }
}
