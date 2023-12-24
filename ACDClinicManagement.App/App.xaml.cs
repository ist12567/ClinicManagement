using ACDClinicManagement.AppHelpers.CommonHelpers;
using System.Reflection;
using System.Windows;

namespace ACDClinicManagement.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            SecurityHelper.EncodeConfigFile(Assembly.GetExecutingAssembly());
        }
    }
}
