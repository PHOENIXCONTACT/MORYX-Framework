// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Moryx.Orders.Documents
{
    /// <summary>
    /// Document which the file source is the web
    /// </summary>
    [DataContract]
    public class WebDocument : Document
    {
        /// <summary>
        /// Url to download the file
        /// </summary>
        [DataMember]
        public string Url { get; set; }

        private static readonly HttpClient _client = new();

        /// <summary>
        /// Default constructor
        /// </summary>
        public WebDocument()
        {
        }

        /// <summary>
        /// Constructor to create a web document
        /// </summary>
        public WebDocument(string number, short revision, string url) : base(number, revision)
        {
            Url = url;
        }

        /// <inheritdoc />
        public override Stream GetStream()
        {
            var response = _client.GetAsync(Url).Result;
            ContentType = response.Content.Headers.ContentType.MediaType;

            return response.Content.ReadAsStreamAsync().Result;
        }
    }
}