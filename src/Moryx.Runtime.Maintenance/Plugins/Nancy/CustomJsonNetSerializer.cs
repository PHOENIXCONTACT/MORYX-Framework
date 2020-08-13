using System;
using System.Collections.Generic;
using System.IO;
using Nancy;
using Nancy.Responses.Negotiation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Moryx.Runtime.Maintenance.Plugins
{
    public sealed class CustomJsonNetSerializer : JsonSerializer, ISerializer
    {
        public CustomJsonNetSerializer()
        {
            ContractResolver = new DefaultContractResolver();
            DateFormatHandling = DateFormatHandling.IsoDateFormat;
            Formatting = Formatting.None;
        }
        public bool CanSerialize(MediaRange mediaRange)
        {
            return mediaRange.ToString().Equals("application/json", StringComparison.OrdinalIgnoreCase);
        }

        public void Serialize<TModel>(MediaRange mediaRange, TModel model, Stream outputStream)
        {
            using (var streamWriter = new StreamWriter(outputStream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                Serialize(jsonWriter, model);
            }
        }

        public IEnumerable<string> Extensions { get { yield return "json"; } }
    }
}