/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ActionType } from "../../common/redux/Types";
import { FailureBehaviour } from "../models/FailureBehaviour";
import { ModuleServerModuleState } from "../models/ModuleServerModuleState";
import { ModuleStartBehaviour } from "../models/ModuleStartBehaviour";
import NotificationModel from "../models/NotificationModel";
import ServerModuleModel from "../models/ServerModuleModel";

export const UPDATE_MODULES = "UPDATE_MODULES";
export const UPDATE_HEALTHSTATE = "UPDATE_HEALTHSTATE";
export const UPDATE_NOTIFICATIONS = "UPDATE_NOTIFICATIONS";
export const UPDATE_START_BEHAVIOUR = "UPDATE_START_BEHAVIOUR";
export const UPDATE_FAILURE_BEHAVIOUR = "UPDATE_FAILURE_BEHAVIOUR";

export function updateModules(modules: ServerModuleModel[]): ActionType<ServerModuleModel[]> {
    return { type: UPDATE_MODULES, payload: modules };
}

export function updateHealthState(moduleName: string, healthState: ModuleServerModuleState): ActionType<{ moduleName: string, healthState: ModuleServerModuleState }> {
    return { type: UPDATE_HEALTHSTATE, payload: { moduleName, healthState } };
}

export function updateNotifications(moduleName: string, notifications: NotificationModel[]): ActionType<{ moduleName: string, notifications: NotificationModel[] }> {
    return { type: UPDATE_NOTIFICATIONS, payload: { moduleName, notifications } };
}

export function updateStartBehaviour(moduleName: string, startBehaviour: ModuleStartBehaviour): ActionType<{ moduleName: string, startBehaviour: ModuleStartBehaviour }> {
    return { type: UPDATE_START_BEHAVIOUR, payload: { moduleName, startBehaviour } };
}

export function updateFailureBehaviour(moduleName: string, failureBehaviour: FailureBehaviour): ActionType<{ moduleName: string, failureBehaviour: FailureBehaviour }> {
    return { type: UPDATE_FAILURE_BEHAVIOUR, payload: { moduleName, failureBehaviour } };
}
