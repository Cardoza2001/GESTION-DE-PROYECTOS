using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace SistemaEmpresa.Services
{
    public static class DialogService
    {
        private static XamlRoot? _xamlRoot;
        private static bool _dialogOpen;

        public static void Initialize(XamlRoot root)
        {
            _xamlRoot = root;
        }

        public static async Task ShowMessage(string mensaje, string titulo = "Sistema")
        {
            if (_xamlRoot == null || _dialogOpen)
                return;

            _dialogOpen = true;

            try
            {
                var dialog = new ContentDialog
                {
                    Title = titulo,
                    Content = mensaje,
                    CloseButtonText = "OK",
                    XamlRoot = _xamlRoot
                };

                await dialog.ShowAsync();
            }
            finally
            {
                _dialogOpen = false;
            }
        }

        public static async Task<bool> Confirm(string mensaje, string titulo = "Confirmación")
        {
            if (_xamlRoot == null || _dialogOpen)
                return false;

            _dialogOpen = true;

            try
            {
                var dialog = new ContentDialog
                {
                    Title = titulo,
                    Content = mensaje,
                    PrimaryButtonText = "Sí",
                    CloseButtonText = "No",
                    XamlRoot = _xamlRoot
                };

                var result = await dialog.ShowAsync();
                return result == ContentDialogResult.Primary;
            }
            finally
            {
                _dialogOpen = false;
            }
        }
    }
}