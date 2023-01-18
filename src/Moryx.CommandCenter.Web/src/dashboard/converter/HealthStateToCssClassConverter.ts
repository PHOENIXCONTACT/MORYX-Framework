/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { ModuleServerModuleState } from "../../modules/models/ModuleServerModuleState";

export interface ForegroundBackgroundCssClass {
    Foreground: string;
    Background: string;
}

export class HealthStateToCssClassConverter {
    public static Convert(healthState: ModuleServerModuleState): ForegroundBackgroundCssClass {
        switch (healthState) {
            case ModuleServerModuleState.Failure: {
                return { Background: "danger", Foreground: "normal" };
            }
            case ModuleServerModuleState.Initializing: {
                return { Background: "info", Foreground: "normal" };
            }
            case ModuleServerModuleState.Ready: {
                return { Background: "secondary", Foreground: "composite" };
            }
            case ModuleServerModuleState.Running: {
                return { Background: "success", Foreground: "normal" };
            }
            case ModuleServerModuleState.Starting: {
                return { Background: "info", Foreground: "normal" };
            }
            case ModuleServerModuleState.Stopping: {
                return { Background: "info", Foreground: "normal" };
            }
            case ModuleServerModuleState.Stopped: {
                return { Background: "light", Foreground: "composite" };
            }
            default: {
                return { Background: "danger", Foreground: "normal" };
            }
        }
    }
}
