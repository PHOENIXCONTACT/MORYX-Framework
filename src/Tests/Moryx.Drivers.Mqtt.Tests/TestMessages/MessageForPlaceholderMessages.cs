// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;

namespace Moryx.Drivers.Mqtt.Tests.TestMessages;

public class MessageForPlaceholderMessages
{
    public string PcName { get; set; }
    public int AdapterNumber { get; set; }

    public ProductIdentity Identity { get; set; }
    public int Value { get; set; }

    public TestClass ClassProperty { get; set; }
}

public class TestClass
{
    public string Test { get; set; }
}