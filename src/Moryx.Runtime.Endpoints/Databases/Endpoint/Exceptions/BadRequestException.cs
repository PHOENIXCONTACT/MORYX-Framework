// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Exceptions
{
    public class BadRequestException : Exception
    {
        public string[] Errors { get; }

        public BadRequestException(string[] errors) : base(string.Join("\n", errors))
        {
            Errors = errors;
        }
    }
}

