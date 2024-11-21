/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

require("../../types/constants");
import CommonRestClient from "../api/CommonRestClient";
import { UPDATE_IS_CONNECTED, UPDATE_NOTIFICATION_INSTANCE, UPDATE_SERVER_TIME, UPDATE_SHOW_WAIT_DIALOG } from "./CommonActions";
import { ActionType } from "./Types";

export interface CommonState {
    IsConnected: boolean;
    ServerTime: string;
    RestClient: CommonRestClient;
    ShowWaitDialog: boolean;
}

export const initialCommonState: CommonState = {
    IsConnected: true,
    RestClient: new CommonRestClient(BASE_URL),
    ServerTime: "",
    ShowWaitDialog: false,
};

export function getCommonReducer(state: CommonState = initialCommonState, action: ActionType<{}>): CommonState {
    switch (action.type) {
        case UPDATE_SERVER_TIME: {
            return { ...state, ServerTime: action.payload as string };
        }
        case UPDATE_IS_CONNECTED: {
            return { ...state, IsConnected: action.payload as boolean };
        }
        case UPDATE_SHOW_WAIT_DIALOG: {
            return { ...state, ShowWaitDialog: action.payload as boolean };
        }
    }
    return state;
}
