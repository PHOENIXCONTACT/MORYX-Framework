// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace Moryx.Orders.Endpoints
{
    [Display(Name = "MORYX Orders Endpoint Source", Description = "Source for operations created at the MORYX Orders Endpoint")]
    internal class ClientOperationSource : IOperationSource
    {
        public string Type => typeof(ClientOperationSource).ToString();

        /// <summary>
        /// Creates a new instance of the <see cref="ClientOperationSource"/>
        /// </summary>
        public ClientOperationSource()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ClientOperationSource"/>
        /// </summary>
        /// <param name="clientId">Id of the client which was creating the operation</param>
        public ClientOperationSource(string clientId)
        {
            ClientId = clientId;
        }

        /// <summary>
        /// Identifier of the client which has created the operation
        /// </summary>
        [Display(Name = "Client ID", Description = "The ID of client that created the operation.")]
        public string ClientId { get; set; }
    }
}

