// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.DependentTestModule
{
    public interface ISimpleHelloWorldWcfSvcMgr
    {
        string Hello(string name);
        string Throw(string name);
    }
}
