// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Media.Endpoints.Model;

namespace Moryx.Media.Endpoints
{
    internal static class MediaModelConverter
    {
        public static ContentDescriptorModel ConvertContent(ContentDescriptor contentDescriptor)
        {
            if (contentDescriptor == null)
                return null;

            var variants = contentDescriptor.Variants.ToList();
            return new ContentDescriptorModel
            {
                Id = contentDescriptor.Id,
                Name = contentDescriptor.Name,
                Variants = variants.ToArray(),
                Master = contentDescriptor.GetMaster()
            };
        }
    }
}

