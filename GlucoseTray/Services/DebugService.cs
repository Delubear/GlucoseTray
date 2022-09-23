using System;
using System.Windows;

namespace GlucoseTray.Services
{
    internal static class DebugService
    {
        internal static void ShowDebugAlert(Exception ex, string message)
        {
            MessageBox.Show(ex?.Message + ex?.InnerException?.Message, message, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
