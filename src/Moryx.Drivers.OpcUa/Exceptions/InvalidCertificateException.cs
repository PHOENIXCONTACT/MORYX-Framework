// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Drivers.OpcUa.Exceptions;

[Serializable]
internal class InvalidCertificateException : Exception
{
    public InvalidCertificateException()
    {
    }

    public InvalidCertificateException(string message) : base(message)
    {
    }

    public InvalidCertificateException(string message, Exception innerException) : base(message, innerException)
    {
    }
}