// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Media.Endpoints.Model;

namespace Moryx.Media.Endpoints
{
    public static class MediaModelConverter
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

