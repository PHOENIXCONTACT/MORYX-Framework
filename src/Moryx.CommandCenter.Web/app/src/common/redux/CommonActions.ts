/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ActionType } from "./Types";

export const UPDATE_SERVER_TIME = "UPDATE_SERVER_TIME";
export const UPDATE_IS_CONNECTED = "UPDATE_IS_CONNECTED";
export const UPDATE_NOTIFICATION_INSTANCE = "UPDATE_NOTIFICATION_INSTANCE";
export const UPDATE_SHOW_WAIT_DIALOG = "UPDATE_SHOW_WAIT_DIALOG";

export function updateServerTime(newTime: string): ActionType<string> {
    return { type: UPDATE_SERVER_TIME, payload: newTime };
}

export function updateIsConnected(isConnected: boolean): ActionType<boolean> {
    return { type: UPDATE_IS_CONNECTED, payload: isConnected };
}

export function updateShowWaitDialog(showWaitDialog: boolean): ActionType<boolean> {
    return { type: UPDATE_SHOW_WAIT_DIALOG, payload: showWaitDialog };
}
