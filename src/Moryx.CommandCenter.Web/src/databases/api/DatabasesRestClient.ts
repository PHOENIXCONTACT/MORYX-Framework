/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import InvocationResponse from "../../common/api/responses/InvocationResponse";
import RestClientBase from "../../common/api/RestClientBase";
import DatabaseConfigModel from "../models/DatabaseConfigModel";
import DataModel from "../models/DataModel";
import ListOfDataModels from "../models/ListOfDataModels";
import ExecuteSetupRequest from "./requests/ExecuteSetupRequest";
import RestoreDatabaseRequest from "./requests/RestoreDatabaseRequest";
import DatabaseUpdateSummary from "./responses/DatabaseUpdateSummary";
import ResponseModel from "./responses/ResponseModel";
import TestConnectionResponse from "./responses/TestConnectionResponse";

const ROOT_PATH = "/databases";
const MODEL_PATH = ROOT_PATH + "/{target}";

export default class DatabasesRestClient extends RestClientBase {
    public databaseModels(): Promise<ResponseModel<ListOfDataModels>> {
        return this.get<ResponseModel<ListOfDataModels>>(ROOT_PATH, new ResponseModel<ListOfDataModels>());
    }

    public deleteAllDatabaseModels(): Promise<InvocationResponse> {
        return this.deleteNoBody<InvocationResponse>(ROOT_PATH, new InvocationResponse());
    }

    public databaseModel(targetModel: string): Promise<ResponseModel<DataModel>> {
        return this.get<ResponseModel<DataModel>>(DatabasesRestClient.pathTo(targetModel), new ResponseModel<DataModel>());
    }

    public saveDatabaseConfig(request: DatabaseConfigModel, targetModel: string): Promise<ResponseModel<DatabaseConfigModel>> {
        return this.post<DatabaseConfigModel, ResponseModel<DatabaseConfigModel>>(DatabasesRestClient.pathTo(targetModel, "/config"), request, new ResponseModel<DatabaseConfigModel>());
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
        return this.post<DatabaseConfigModel, DatabaseUpdateSummary>(DatabasesRestClient.pathTo(targetModel, `/${migrationName}/migrate`), request, new DatabaseUpdateSummary());
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
