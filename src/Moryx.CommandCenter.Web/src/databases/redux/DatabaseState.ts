/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

require("../../types/constants");
import { ActionType } from "../../common/redux/Types";
import DatabasesRestClient from "../api/DatabasesRestClient";
import DatabaseAndConfigurators from "../models/DatabaseAndConfigurators";
import DataModel from "../models/DataModel";
import { UPDATE_DATABASE_CONFIG, UPDATE_DATABASE_CONFIGS } from "./DatabaseActions";

export interface DatabaseState {
    RestClient: DatabasesRestClient;
    Databases: DatabaseAndConfigurators[];
}

export const initialDatabaseState: DatabaseState = {
    Databases: [],
    RestClient: new DatabasesRestClient(BASE_URL),
};

export function getDatabaseReducer(state: DatabaseState = initialDatabaseState, action: ActionType<{}>): DatabaseState {
  switch (action.type) {
    case UPDATE_DATABASE_CONFIGS: {
        return { ...state, Databases: action.payload as DatabaseAndConfigurators[] };
    }
    case UPDATE_DATABASE_CONFIG: {
        const database = action.payload as DataModel;

        const databases = state.Databases.map((dbAndConfig) => {
            if (dbAndConfig.database.targetModel === database.targetModel) {
                dbAndConfig.database = database;
            }
            return dbAndConfig;
        });

        console.log("UPDATE_DATABASE_CONFIG", database);
        return {
            ...state,
            Databases: databases,
         };
    }
  }
  return state;
}
