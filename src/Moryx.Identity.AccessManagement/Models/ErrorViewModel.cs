// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Identity.AccessManagement.Models
{
    /// <summary>
    /// View model to display erros.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// The id of the errornous request.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// True if the <see cref="RequestId"/> exists; false otherwise.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

