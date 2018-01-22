using System;
using Caliburn.Micro;
using Marvin.ClientFramework.Dialog;

namespace Marvin.AbstractionLayer.UI.Tests
{
    public class DialogManagerMock : IDialogManager
    {
        public void ShowDialog<T>(T dialogViewModel) where T : IScreen
        {
            
        }

        public void ShowDialog<T>(T dialogViewModel, Action<T> callback) where T : IScreen
        {
            
        }

        public void ShowMessageBox(string message, string title, MessageBoxOptions options, MessageBoxImage image, Action<IMessageBox> callback)
        {
            
        }

        public void ShowMessageBox(string message, string title, MessageBoxOptions options, MessageBoxImage image)
        {
            
        }

        public void ShowMessageBox(string message, string title, Action<IMessageBox> callback)
        {
            
        }

        public void ShowMessageBox(string message, string title)
        {
            
        }
    }
}