using Microsoft.UI.Xaml;
using SistemaEmpresa.Views.Login;
using SistemaEmpresa.Database;

namespace SistemaEmpresa
{
    public partial class App : Application
    {
        private Window? m_window;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            DatabaseInitializer.Initialize();
            m_window = new LoginWindow();
            m_window.Activate();
        }
    }
}