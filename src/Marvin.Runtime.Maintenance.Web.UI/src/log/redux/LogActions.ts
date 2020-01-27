import { ActionType } from "../../common/redux/Types";
import LoggerModel from "../models/LoggerModel";

export const UPDATE_LOGGERS = "UPDATE_LOGGERS";

export function updateLoggers(loggers: LoggerModel[]): ActionType<LoggerModel[]> {
    return { type: UPDATE_LOGGERS, payload: loggers };
}
