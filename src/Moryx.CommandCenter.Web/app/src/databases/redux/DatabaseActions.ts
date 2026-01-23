/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ActionType } from "../../common/redux/Types";
import DatabaseAndConfigurators from "../models/DatabaseAndConfigurators";
import DataModel from "../models/DataModel";

export const UPDATE_DATABASE_CONFIGS = "UPDATE_DATABASE_CONFIGS";
export const UPDATE_DATABASE_CONFIG = "UPDATE_DATABASE_CONFIG";

export function updateDatabases(databaseAndConfigurators: DatabaseAndConfigurators[]): ActionType<DatabaseAndConfigurators[]> {
    return { type: UPDATE_DATABASE_CONFIGS, payload: databaseAndConfigurators };
}

export function updateDatabaseConfig(config: DataModel): ActionType<DataModel> {
    return { type: UPDATE_DATABASE_CONFIG, payload: config };
}
