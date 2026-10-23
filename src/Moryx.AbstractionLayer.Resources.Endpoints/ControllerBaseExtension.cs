using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Resources.Endpoints.Properties;
using Moryx.AspNetCore;

public static class ControllerBaseExtension
 {

    /// <summary>404 - No resource exists with the given id.</summary>
    public static ObjectResult ResourceNotFound(this ControllerBase controller, long id)
        => controller.Problem().NotFound(
            Strings.ResourceModificationController_ResourceNotFound_Title,
            string.Format(CultureInfo.CurrentCulture,
                Strings.ResourceModificationController_ResourceNotFound_ById_Message, id));

    /// <summary>404 - No resource type exists with the given name.</summary>
    public static ObjectResult ResourceTypeNotFound(this ControllerBase controller, string type)
        => controller.Problem().NotFound(
            Strings.ResourceModificationController_ResourceTypeNotFound_Title,
            string.Format(CultureInfo.CurrentCulture,
                Strings.ResourceModificationController_ResourceTypeNotFound_Message, type));

    /// <summary>404 - The named method does not exist on the resource.</summary>
    public static ObjectResult MethodNotFound(this ControllerBase controller, string method, long id)
        => controller.Problem().NotFound(
            Strings.ResourceModificationController_MethodNotFound_Title,
            string.Format(CultureInfo.CurrentCulture,
                Strings.ResourceModificationController_MethodNotFound_Message, method, id));

    /// <summary>409 - A resource with the given id already exists.</summary>
    public static ObjectResult ResourceAlreadyExists(this ControllerBase controller, long id)
        => controller.Problem().Conflict(
            Strings.ResourceModificationController_ResourceAlreadyExists_Title,
            string.Format(CultureInfo.CurrentCulture,
                Strings.ResourceModificationController_ResourceAlreadyExists_Message, id));

    /// <summary>409 - The resource cannot be removed because other resources still reference it.</summary>
    public static ObjectResult ResourceReferenceConflict(this ControllerBase controller, long id)
        => controller.Problem().Conflict(
            Strings.ResourceModificationController_ResourceConflict_Title,
            string.Format(CultureInfo.CurrentCulture,
                Strings.ResourceModificationController_ResourceConflict_Message, id));

    /// <summary>422 - Method invocation raised an unhandled exception.</summary>
    public static ObjectResult MethodFailed(this ControllerBase controller, string method)
        => controller.Problem().UnprocessableContent(
            Strings.ResourceModificationController_MethodFailed_Title,
            string.Format(CultureInfo.CurrentCulture,
                Strings.ResourceModificationController_MethodFailed_Message, method));

    /// <summary>400 - Generic invalid argument response used for expected validation errors.</summary>
    public static ObjectResult InvalidArgument(this ControllerBase controller)
        => controller.Problem().BadRequest(
            Strings.ResourceModificationController_InvalidArgument_Title,
            Strings.ResourceModificationController_InvalidArgument_Message);
}
