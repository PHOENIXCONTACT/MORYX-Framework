/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
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
                return { color: "#e5ac02" };
            }
            case Serverity.Error: {
                return { color: "#d32f2f" };
            }
            case Serverity.Fatal: {
                return { color: "#d32f2f" };
            }
            default: {
                return { color: "black" };
            }
        }
    }
}
