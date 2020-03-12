// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Tools.Wcf;

namespace Marvin.TestModule
{
    public interface IHelloWorldWcfSvcMgr : IWcfServiceManager
    {
        string Hello(string name);
        string Throw(string name);

        void TriggerHelloCallback(string name);
        void HelloCallback(string name);
        void TriggerThrowCallback(string name);
    }
}
