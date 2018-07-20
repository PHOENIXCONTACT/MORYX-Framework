import * as React from "react";
import { ModuleNotificationType } from "../models/ModuleNotificationType";

export class ModuleNotificationTypeToCssClassConverter {
    public static Convert(healthState: ModuleNotificationType): React.CSSProperties {
        switch (healthState) {
            case ModuleNotificationType.Info: {
                return { color: "black" };
            }
            case ModuleNotificationType.Warning: {
                return { color: "yellow" };
            }
            case ModuleNotificationType.Failure: {
                return { color: "red" };
            }
            default: {
                return { color: "black" };
            }
        }
    }
}
