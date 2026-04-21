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
            this.RequestedTheme = ApplicationTheme.Light;
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            DatabaseInitializer.Initialize();

            m_window = new LoginWindow();

            if (m_window.Content is FrameworkElement root)
            {
                root.RequestedTheme = ElementTheme.Light;
            }

            m_window.Activate();
        }
    }
}