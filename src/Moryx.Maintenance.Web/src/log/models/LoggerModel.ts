/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/
import { LogLevel } from "./LogLevel";

export default class LoggerModel {
    public name: string;
    public activeLevel: LogLevel;
    public childLogger: LoggerModel[];
    public parent: LoggerModel;

    public static shortLoggerName(logger: LoggerModel): string {
        const splittedLoggerPath = logger.name.split(".");
        return splittedLoggerPath[splittedLoggerPath.length - 1];
    }
}
