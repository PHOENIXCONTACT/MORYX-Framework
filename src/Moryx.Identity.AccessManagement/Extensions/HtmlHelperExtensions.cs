// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Moryx.Identity.AccessManagement;

static class HtmlHelperExtensions
{
    private const string _selectedClass = "moryx-selected";

    extension(IHtmlHelper html)
    {
        public string IsActionSelected(string controllerName, params string[] actionNames)
        {
            string contextController = (string)html.ViewContext.RouteData.Values["controller"];
            string contextAction = (string)html.ViewContext.RouteData.Values["action"];

            if (!string.Equals(contextController, controllerName, StringComparison.CurrentCultureIgnoreCase))
                return string.Empty;

            foreach (string action in actionNames)
            {
                if (string.Equals(contextAction, action, StringComparison.CurrentCultureIgnoreCase))
                    return _selectedClass;
            }
            return string.Empty;
        }

        public string IsControllerSelected(params string[] controllerNames)
        {
            string contextController = (string)html.ViewContext.RouteData.Values["controller"];

            foreach (string controller in controllerNames)
            {
                if (string.Equals(contextController, controller, StringComparison.CurrentCultureIgnoreCase))
                    return _selectedClass;
            }
            return string.Empty;
        }
    }
}