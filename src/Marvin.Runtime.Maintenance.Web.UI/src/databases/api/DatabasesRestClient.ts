/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import InvocationResponse from "../../common/api/responses/InvocationResponse";
import RestClientBase from "../../common/api/RestClientBase";
import BackupModel from "../models/BackupModel";
import DatabaseConfigModel from "../models/DatabaseConfigModel";
import DataModel from "../models/DataModel";
import { TestConnectionResult } from "../models/TestConnectionResult";
import ExecuteScriptRequest from "./requests/ExecuteScriptRequest";
import ExecuteSetupRequest from "./requests/ExecuteSetupRequest";
import RestoreDatabaseRequest from "./requests/RestoreDatabaseRequest";
import DatabaseUpdateSummary from "./responses/DatabaseUpdateSummary";
import TestConnectionResponse from "./responses/TestConnectionResponse";

export default class DatabasesRestClient extends RestClientBase {
    public databaseModels(): Promise<DataModel[]> {
        return this.get<DataModel[]>("/DatabaseMaintenance/models", []);
    }

    public deleteAllDatabaseModels(): Promise<InvocationResponse> {
        return this.deleteNoBody<InvocationResponse>("/DatabaseMaintenance/models", new InvocationResponse());
    }

    public databaseModel(targetModel: string): Promise<DataModel> {
        return this.get<DataModel>("/DatabaseMaintenance/models/"  + targetModel, new DataModel());
    }

    public saveDatabaseConfig(request: DatabaseConfigModel, targetModel: string): Promise<Response> {
        return this.post<DatabaseConfigModel, Response>("/DatabaseMaintenance/models/" + targetModel + "/config", request, new Response());
    }

    public testDatabaseConfig(request: DatabaseConfigModel, targetModel: string): Promise<TestConnectionResponse> {
        return this.post<DatabaseConfigModel, TestConnectionResponse>("/DatabaseMaintenance/models/" + targetModel + "/config/test", request, new TestConnectionResponse());
    }

    public createDatabase(request: DatabaseConfigModel, targetModel: string): Promise<InvocationResponse> {
        return this.post<DatabaseConfigModel, InvocationResponse>("/DatabaseMaintenance/models/" + targetModel + "/create", request, new InvocationResponse());
    }

    public eraseDatabase(request: DatabaseConfigModel, targetModel: string): Promise<InvocationResponse> {
        return this.delete<DatabaseConfigModel, InvocationResponse>("/DatabaseMaintenance/models/" + targetModel, request, new InvocationResponse());
    }

    public dumpDatabase(request: DatabaseConfigModel, targetModel: string): Promise<InvocationResponse> {
        return this.post<DatabaseConfigModel, InvocationResponse>("/DatabaseMaintenance/models/" + targetModel + "/dump", request, new InvocationResponse());
    }

    public restoreDatabase(request: RestoreDatabaseRequest, targetModel: string): Promise<InvocationResponse> {
        return this.post<RestoreDatabaseRequest, InvocationResponse>("/DatabaseMaintenance/models/" + targetModel + "/restore", request, new InvocationResponse());
    }

    public applyMigration(targetModel: string, migrationName: string, request: DatabaseConfigModel): Promise<DatabaseUpdateSummary> {
        return this.post<DatabaseConfigModel, DatabaseUpdateSummary>("/DatabaseMaintenance/models/" + targetModel + "/" + migrationName + "/migrate", request, new DatabaseUpdateSummary());
    }

    public rollbackDatabase(targetModel: string, request: DatabaseConfigModel): Promise<InvocationResponse> {
        return this.post<DatabaseConfigModel, InvocationResponse>("/DatabaseMaintenance/models/" + targetModel + "/rollback", request, new InvocationResponse());
    }

    public executeSetup(targetModel: string, request: ExecuteSetupRequest): Promise<InvocationResponse> {
        return this.post<ExecuteSetupRequest, InvocationResponse>("/DatabaseMaintenance/models/"  + targetModel + "/setup", request, new InvocationResponse());
    }

    public executeScript(targetModel: string, request: ExecuteScriptRequest): Promise<InvocationResponse> {
        return this.post<ExecuteScriptRequest, InvocationResponse>("/DatabaseMaintenance/models/"  + targetModel + "/script", request, new InvocationResponse());
    }
}
