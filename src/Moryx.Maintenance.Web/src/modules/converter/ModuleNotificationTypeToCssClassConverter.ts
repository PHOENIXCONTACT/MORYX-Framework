/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { Serverity } from "../models/Severity";

export class ModuleNotificationTypeToCssClassConverter {
    public static Convert(healthState: Serverity): React.CSSProperties {
        switch (healthState) {
            case Serverity.Info: {
                return { color: "black" };
            }
            case Serverity.Warning: {
                return { color: "orange" };
            }
            case Serverity.Error: {
                return { color: "red" };
            }
            case Serverity.Fatal: {
                return { color: "purple" };
            }
            default: {
                return { color: "black" };
            }
        }
    }
}
