using Reader.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Reader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Repository.Instance = RepositoryFactory.GetRepositoryInstance();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Error");
            File.WriteAllText($"ErrorLog_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm")}", GetErrorText(e.Exception));
            e.Handled = true;
        }

        private string GetErrorText(Exception exception)
        {
            if (exception == null) return string.Empty;
            string br = Environment.NewLine;
            return $"{exception.Message}{br}{exception.StackTrace}{br}{GetErrorText(exception.InnerException)}";
            
        }
    }
}
