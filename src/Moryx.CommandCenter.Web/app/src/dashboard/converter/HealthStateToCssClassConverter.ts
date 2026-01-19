/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ChipPropsColorOverrides } from "@mui/material/Chip";
import * as React from "react";
import { ModuleServerModuleState } from "../../modules/models/ModuleServerModuleState";

export interface ForegroundBackgroundCssClass {
    Foreground: string;
    Background: string;
}

export class HealthStateToCssClassConverter {
    public static Color(healthState: ModuleServerModuleState): "default" | "primary" | "secondary" | "error" | "info" | "success" | "warning" {
        switch (healthState) {
            case ModuleServerModuleState.Failure: {
                return "error";
            }
            case ModuleServerModuleState.Initializing: {
                return "info";
            }
            case ModuleServerModuleState.Ready: {
                return "default";
            }
            case ModuleServerModuleState.Running: {
                return "success";
            }
            case ModuleServerModuleState.Starting: {
                return "info";
            }
            case ModuleServerModuleState.Stopping: {
                return "info";
            }
            case ModuleServerModuleState.Stopped: {
                return "warning";
            }
            default: {
                return "error";
            }
        }
    }
}
