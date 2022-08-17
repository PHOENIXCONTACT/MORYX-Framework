/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

const TraceImg = require("../images/Trace.png");
const DebugImg = require("../images/Debug.png");
const InfoImg = require("../images/Info.png");
const WarningImg = require("../images/Warning.png");
const ErrorImg = require("../images/Error.png");
const FatalImg = require("../images/Fatal.png");
import * as React from "react";
import LoggerModel from "../models/LoggerModel";
import { LogLevel } from "../models/LogLevel";

interface LogMenuItemContentPropsModel {
    Logger: LoggerModel;
    onActiveLogLevelChange(e: React.FormEvent<HTMLElement>, l: LoggerModel): void;
    onLabelClicked(l: LoggerModel): void;
}

export default class LogMenuItemContent extends React.Component<LogMenuItemContentPropsModel, {}> {

    public render(): React.ReactNode {
        let img: any = TraceImg;
        switch (this.props.Logger.ActiveLevel) {
            case LogLevel.Trace: img = TraceImg; break;
            case LogLevel.Debug: img = DebugImg; break;
            case LogLevel.Info: img = InfoImg; break;
            case LogLevel.Warning: img = WarningImg; break;
            case LogLevel.Error: img = ErrorImg; break;
            case LogLevel.Fatal: img = FatalImg;
        }

        return (
            <div>
                <select className="log-select" style={{backgroundImage: `url(${img})`}}
                        onChange={(e: React.FormEvent<HTMLElement>) => this.props.onActiveLogLevelChange(e, this.props.Logger)}
                        value={this.props.Logger.ActiveLevel}>
                    <option value={LogLevel.Trace}>Trace</option>
                    <option value={LogLevel.Debug}>Debug</option>
                    <option value={LogLevel.Info}>Info</option>
                    <option value={LogLevel.Warning}>Warning</option>
                    <option value={LogLevel.Error}>Error</option>
                    <option value={LogLevel.Fatal}>Fatal</option>
                </select>
                <span className="font-italic"
                      style={{wordBreak: "break-all"}}
                      onClick={(e: React.MouseEvent<HTMLElement>) => this.props.onLabelClicked(this.props.Logger)}>
                    {LoggerModel.shortLoggerName(this.props.Logger)}
                </span>
            </div>
        );
    }
}
