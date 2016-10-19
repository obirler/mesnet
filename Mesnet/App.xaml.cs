using System.Windows;
using Mesnet.Classes.Tools;

namespace Mesnet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Logger.CloseLogger();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logger.InitializeLogger();
        }
    }
}
