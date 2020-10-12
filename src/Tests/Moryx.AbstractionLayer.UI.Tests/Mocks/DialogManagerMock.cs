// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Moryx.ClientFramework.Dialog;

namespace Moryx.AbstractionLayer.UI.Tests
{
    public class DialogManagerMock : IDialogManager
    {
        public void ShowDialog<T>(T dialogViewModel) where T : IScreen
        {

        }

        public Task ShowDialogAsync<T>(T dialogViewModel) where T : IScreen
        {
            return Task.FromResult(true);
        }

        public void ShowDialog<T>(T dialogViewModel, Action<T> callback) where T : IScreen
        {

        }

        public Task<MessageBoxOptions> ShowMessageBoxAsync(string message, string title, MessageBoxOptions options, MessageBoxImage image)
        {
            return Task.FromResult(MessageBoxOptions.Ok);
        }

        public void ShowMessageBox(string message, string title, MessageBoxOptions options, MessageBoxImage image, Action<IMessageBox> callback)
        {

        }

        public void ShowMessageBox(string message, string title, MessageBoxOptions options, MessageBoxImage image)
        {

        }

        public Task ShowMessageBoxAsync(string message, string title)
        {
            return Task.FromResult(true);
        }

        public void ShowMessageBox(string message, string title, Action<IMessageBox> callback)
        {

        }

        public void ShowMessageBox(string message, string title)
        {

        }
    }
}
