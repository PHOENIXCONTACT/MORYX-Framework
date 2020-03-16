/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import ApplicationInformationResponse from "../../common/api/responses/ApplicationInformationResponse";
import ApplicationLoadResponse from "../../common/api/responses/ApplicationLoadResponse";
import HostInformationResponse from "../../common/api/responses/HostInformationResponse";
import SystemLoadResponse from "../../common/api/responses/SystemLoadResponse";
import { ActionType } from "../../common/redux/Types";

export const UPDATE_APPLICATION_INFO = "UPDATE_APPLICATION_INFO";
export const UPDATE_HOST_INFO = "UPDATE_HOST_INFO";
export const UPDATE_APPLICATION_LOAD = "UPDATE_APPLICATION_LOAD";
export const UPDATE_SYSTEM_LOAD = "UPDATE_SYSTEM_LOAD";

export function updateApplicationInfo(applicationInfo: ApplicationInformationResponse): ActionType<ApplicationInformationResponse> {
    return { type: UPDATE_APPLICATION_INFO, payload: applicationInfo };
}

export function updateHostInfo(hostInfo: HostInformationResponse): ActionType<HostInformationResponse> {
    return { type: UPDATE_HOST_INFO, payload: hostInfo };
}

export function updateApplicationLoad(applicationLoad: ApplicationLoadResponse): ActionType<ApplicationLoadResponse> {
    return { type: UPDATE_APPLICATION_LOAD, payload: applicationLoad };
}

export function updateSystemLoad(systemLoad: SystemLoadResponse): ActionType<SystemLoadResponse> {
    return { type: UPDATE_SYSTEM_LOAD, payload: systemLoad };
}
