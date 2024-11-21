/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import InvocationResponse from "../../common/api/responses/InvocationResponse";
import RestClientBase from "../../common/api/RestClientBase";
import DatabaseConfigModel from "../models/DatabaseConfigModel";
import DatabasesResponse from "../models/DatabasesResponse";
import DataModel from "../models/DataModel";
import ExecuteSetupRequest from "./requests/ExecuteSetupRequest";
import RestoreDatabaseRequest from "./requests/RestoreDatabaseRequest";
import DatabaseUpdateSummary from "./responses/DatabaseUpdateSummary";
import TestConnectionResponse from "./responses/TestConnectionResponse";

const ROOT_PATH = "/databases";
const MODEL_PATH = ROOT_PATH + "/{target}";

export default class DatabasesRestClient extends RestClientBase {
    public databaseModels(): Promise<DatabasesResponse> {
        return this.get<DatabasesResponse>(ROOT_PATH, new DatabasesResponse());
    }

    public deleteAllDatabaseModels(): Promise<InvocationResponse> {
        return this.deleteNoBody<InvocationResponse>(ROOT_PATH, new InvocationResponse());
    }

    public databaseModel(targetModel: string): Promise<DataModel> {
        return this.get<DataModel>(DatabasesRestClient.pathTo(targetModel), new DataModel());
    }

    public saveDatabaseConfig(request: DatabaseConfigModel, targetModel: string): Promise<DataModel> {
        return this.post<DatabaseConfigModel, DataModel>(DatabasesRestClient.pathTo(targetModel, "/config"), request, new DataModel());
    }

    public testDatabaseConfig(request: DatabaseConfigModel, targetModel: string): Promise<TestConnectionResponse> {
        return this.post<DatabaseConfigModel, TestConnectionResponse>(DatabasesRestClient.pathTo(targetModel, "/config/test"), request, new TestConnectionResponse());
    }

    public createDatabase(request: DatabaseConfigModel, targetModel: string): Promise<InvocationResponse> {
        return this.post<DatabaseConfigModel, InvocationResponse>(DatabasesRestClient.pathTo(targetModel, "/create"), request, new InvocationResponse());
    }

    public eraseDatabase(request: DatabaseConfigModel, targetModel: string): Promise<InvocationResponse> {
        return this.delete<DatabaseConfigModel, InvocationResponse>(DatabasesRestClient.pathTo(targetModel), request, new InvocationResponse());
    }

    public dumpDatabase(request: DatabaseConfigModel, targetModel: string): Promise<InvocationResponse> {
        return this.post<DatabaseConfigModel, InvocationResponse>(DatabasesRestClient.pathTo(targetModel, "/dump"), request, new InvocationResponse());
    }

    public restoreDatabase(request: RestoreDatabaseRequest, targetModel: string): Promise<InvocationResponse> {
        return this.post<RestoreDatabaseRequest, InvocationResponse>(DatabasesRestClient.pathTo(targetModel, "/restore"), request, new InvocationResponse());
    }

    public applyMigration(targetModel: string, migrationName: string, request: DatabaseConfigModel): Promise<DatabaseUpdateSummary> {
        return this.post<DatabaseConfigModel, DatabaseUpdateSummary>(DatabasesRestClient.pathTo(targetModel, `/migrate`), request, new DatabaseUpdateSummary());
    }

    public rollbackDatabase(targetModel: string, request: DatabaseConfigModel): Promise<InvocationResponse> {
        return this.post<DatabaseConfigModel, InvocationResponse>(DatabasesRestClient.pathTo(targetModel, "/rollback"), request, new InvocationResponse());
    }

    public executeSetup(targetModel: string, request: ExecuteSetupRequest): Promise<InvocationResponse> {
        return this.post<ExecuteSetupRequest, InvocationResponse>(DatabasesRestClient.pathTo(targetModel, "/setup"), request, new InvocationResponse());
    }

    private static pathTo(targetModel: string, path: string = ""): string {
        return MODEL_PATH.replace("{target}", targetModel) + path;
    }
}
