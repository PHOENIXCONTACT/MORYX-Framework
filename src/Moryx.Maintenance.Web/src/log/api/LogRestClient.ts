/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/
import InvocationResponse from "../../common/api/responses/InvocationResponse";
import RestClientBase from "../../common/api/RestClientBase";
import LoggerModel from "../models/LoggerModel";
import { LogLevel } from "../models/LogLevel";
import LogMessageModel from "../models/LogMessageModel";
import AddRemoteAppenderRequest from "./requests/AddRemoteAppenderRequest";
import SetLogLevelRequest from "./requests/SetLogLevelRequest";
import AddAppenderResponse from "./responses/AddAppenderResponse";

export default class LogRestClient extends RestClientBase {
    public loggers(): Promise<LoggerModel[]> {
        return this.get<LoggerModel[]>("", []);
    }

    public addRemoteAppender(name: string, minLevel: LogLevel): Promise<AddAppenderResponse> {
        const request = new AddRemoteAppenderRequest();
        request.MinLevel = minLevel;
        request.Name = name;
        return this.post<AddRemoteAppenderRequest, AddAppenderResponse>("/appender", request, new AddAppenderResponse());
    }

    public removeRemoteAppender(appenderId: number): Promise<InvocationResponse> {
        return this.deleteNoBody<InvocationResponse>("/appender/" + appenderId.toString(), new InvocationResponse());
    }

    public messages(appenderId: number): Promise<LogMessageModel[]> {
        return this.get<LogMessageModel[]>("/appender/" + appenderId.toString(), []);
    }

    public logLevel(loggerName: string, level: LogLevel): Promise<InvocationResponse> {
        const request = new SetLogLevelRequest();
        request.Level = level;
        return this.put<SetLogLevelRequest, InvocationResponse>("/logger/" + loggerName + "/loglevel", request, new InvocationResponse());
    }
}
