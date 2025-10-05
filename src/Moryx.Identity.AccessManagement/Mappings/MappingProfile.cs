// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using AutoMapper;
using Moryx.Identity.AccessManagement.Data;
using Moryx.Identity.AccessManagement.Models;

namespace Moryx.Identity.AccessManagement.Mappings
{
    /// <inheritdoc/>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Creates a new mapping profile for the MORYX AccessManagement
        /// </summary>
        public MappingProfile()
        {
            CreateMap<MoryxUserModel, MoryxUser>();

            CreateMap<MoryxUser, MoryxUserModel>();

            CreateMap<MoryxUserRegisterModel, MoryxUser>();

            CreateMap<MoryxUser, MoryxUserResetPasswordModel>();

            CreateMap<MoryxUser, MoryxUserUpdateModel>();

            CreateMap<Permission, PermissionModel>()
                .ForMember(
                    p => p.Roles,
                    m => m.MapFrom(p => p.Roles.Select(r => r.Name).ToArray()));

        }
    }
}

