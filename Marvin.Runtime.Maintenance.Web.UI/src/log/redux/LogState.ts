require("../../types/constants");
import { ActionType } from "../../common/redux/Types";
import LogRestClient from "../api/LogRestClient";
import LoggerModel from "../models/LoggerModel";
import { UPDATE_LOGGERS } from "./LogActions";

export interface LogState {
    RestClient: LogRestClient;
    Loggers: LoggerModel[];
}

export const initialLogState: LogState = {
    Loggers: [],
    RestClient: new LogRestClient(window.location.hostname, parseInt(RESTSERVER_PORT, 10)),
};

export function getLogReducer(state: LogState = initialLogState, action: ActionType<{}>): LogState {
  switch (action.type) {
    case UPDATE_LOGGERS:
    {
        return { ...state, Loggers: action.payload as LoggerModel[] };
    }
  }
  return state;
}
