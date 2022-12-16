/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/
import LoggerModel from "./LoggerModel";
import { LogLevel } from "./LogLevel";

export default class LogMessageModel {
    public logger: LoggerModel;
    public className: string;
    public logLevel: LogLevel;
    public message: string;
    public timestamp: Date;
}
