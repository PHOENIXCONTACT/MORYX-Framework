/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/
import { LogLevel } from "./LogLevel";

export default class LoggerModel {
    public Name: string;
    public ActiveLevel: LogLevel;
    public ChildLogger: LoggerModel[];
    public Parent: LoggerModel;

    public static shortLoggerName(logger: LoggerModel): string {
        const splittedLoggerPath = logger.Name.split(".");
        return splittedLoggerPath[splittedLoggerPath.length - 1];
    }
}
