/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ActionType } from "../../common/redux/Types";
import DataModel from "../models/DataModel";

export const UPDATE_DATABASE_CONFIGS = "UPDATE_DATABASE_CONFIGS";
export const UPDATE_DATABASE_CONFIG = "UPDATE_DATABASE_CONFIG";

export function updateDatabaseConfigs(configs: DataModel[]): ActionType<DataModel[]> {
    return { type: UPDATE_DATABASE_CONFIGS, payload: configs };
}

export function updateDatabaseConfig(config: DataModel): ActionType<DataModel> {
    return { type: UPDATE_DATABASE_CONFIG, payload: config };
}
