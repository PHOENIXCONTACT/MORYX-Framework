import * as React from "react";
import { LogLevel } from "../models/LogLevel";

export default class LogLevelToCssClassConverter {
    public static Convert(logLevel: LogLevel): string {
        switch (logLevel) {
            case LogLevel.Trace: {
                return "#e9e9e9";
            }
            case LogLevel.Debug: {
                return "#d9d9d9";
            }
            case LogLevel.Info: {
                return "white";
            }
            case LogLevel.Warning: {
                return "#ffffcc";
            }
            case LogLevel.Error: {
                return "#ffe6e6";
            }
            case LogLevel.Fatal: {
                return "#ff3333";
            }
            default: {
                return "white";
            }
        }
    }
}
