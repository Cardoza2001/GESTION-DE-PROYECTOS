using Microsoft.UI.Xaml;
using SistemaEmpresa.Views.Login;

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
            m_window = new LoginWindow();
            m_window.Activate();
        }
    }
}