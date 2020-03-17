// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Marvin.ClientFramework.Dialog;

namespace Marvin.AbstractionLayer.UI.Tests
{
    public class DialogManagerMock : IDialogManager
    {
        public void ShowDialog<T>(T dialogViewModel) where T : IScreen
        {
            
        }

        public Task ShowDialogAsync<T>(T dialogViewModel) where T : IScreen
        {
            return Task.CompletedTask;
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
            return Task.CompletedTask;
        }

        public void ShowMessageBox(string message, string title, Action<IMessageBox> callback)
        {
            
        }

        public void ShowMessageBox(string message, string title)
        {
            
        }
    }
}
