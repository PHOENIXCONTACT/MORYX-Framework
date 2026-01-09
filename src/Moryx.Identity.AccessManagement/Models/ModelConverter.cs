// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement.Models;

internal static class ModelConverter
{
    public static MoryxUserUpdateModel GetUserUpdateModelFromUser(MoryxUser user)
    {
        var model = new MoryxUserUpdateModel
        {
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.Firstname,
            LastName = user.LastName
        };

        return model;
    }

    public static MoryxUser GetUserFromUserRegisterModel(MoryxUserRegisterModel userModel)
    {
        var user = new MoryxUser
        {
            UserName = userModel.UserName,
            Email = userModel.Email,
            Firstname = userModel.FirstName,
            LastName = userModel.LastName
        };
        return user;
    }

    public static MoryxUser GetUserFromModel(MoryxUserModel userModel)
    {
        var user = new MoryxUser
        {
            UserName = userModel.UserName,
            Email = userModel.Email,
            Firstname = userModel.FirstName,
            LastName = userModel.LastName
        };
        return user;
    }

    public static MoryxUserModel GetUserModelFromUser(MoryxUser user)
    {
        var model = new MoryxUserModel
        {
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.Firstname,
            LastName = user.LastName
        };
        return model;
    }

    public static PermissionModel GetPermissionModelFromPermission(Permission permission)
    {
        var model = new PermissionModel
        {
            Name = permission.Name,
            Roles = permission.Roles.Select(r => r.Name).ToArray()
        };
        return model;
    }
}
