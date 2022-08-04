/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import RestClientBase from "../../common/api/RestClientBase";
import Config from "../models/Config";
import Entry from "../models/Entry";
import MethodEntry from "../models/MethodEntry";
import { ModuleServerModuleState } from "../models/ModuleServerModuleState";
import NotificationModel from "../models/NotificationModel";
import ServerModuleModel from "../models/ServerModuleModel";
import SaveConfigRequest from "./requests/SaveConfigRequest";

const ROOT_PATH = "/modules";
const MODULE_PATH = ROOT_PATH + "/module/{moduleName}";

export default class ModulesRestClient extends RestClientBase {
    public modules(): Promise<ServerModuleModel[]> {
        return this.get<ServerModuleModel[]>(ROOT_PATH, []);
    }

    public healthState(moduleName: string): Promise<ModuleServerModuleState> {
        return this.get<ModuleServerModuleState>(ModulesRestClient.pathTo(moduleName, "/healthstate"), ModuleServerModuleState.Failure);
    }

    public notifications(moduleName: string): Promise<NotificationModel[]> {
        return this.get<NotificationModel[]>(ModulesRestClient.pathTo(moduleName, "/notifications"), []);
    }

    public startModule(moduleName: string): Promise<Response> {
        return this.postNoBody<Response>(ModulesRestClient.pathTo(moduleName, "/start"), new Response());
    }

    public stopModule(moduleName: string): Promise<Response> {
        return this.postNoBody<Response>(ModulesRestClient.pathTo(moduleName, "/stop"), new Response());
    }

    public reincarnateModule(moduleName: string): Promise<Response> {
        return this.postNoBody<Response>(ModulesRestClient.pathTo(moduleName, "/reincarnate"), new Response());
    }

    public confirmModuleWarning(moduleName: string): Promise<Response> {
        return this.postNoBody<Response>(ModulesRestClient.pathTo(moduleName, "/confirm"), new Response());
    }

    public updateModule(request: ServerModuleModel): Promise<Response> {
        return this.post<ServerModuleModel, Response>(ModulesRestClient.pathTo(request.name), request, new Response());
    }

    public moduleConfig(moduleName: string): Promise<Config> {
        return this.get<Config>(ModulesRestClient.pathTo(moduleName, "/config"), new Config());
    }

    public saveModuleConfig(moduleName: string, request: SaveConfigRequest): Promise<Response> {
        return this.post<SaveConfigRequest, Response>(ModulesRestClient.pathTo(moduleName, "/config"), request, new Response(), ModulesRestClient.entryReplacer);
    }

    public moduleMethods(moduleName: string): Promise<MethodEntry[]> {
        return this.get<MethodEntry[]>(ModulesRestClient.pathTo(moduleName, "/console"), []);
    }

    public invokeMethod(moduleName: string, request: MethodEntry): Promise<Entry> {
        return this.post<MethodEntry, Entry>(ModulesRestClient.pathTo(moduleName, "/console"), request, new Entry(), ModulesRestClient.entryReplacer);
    }

    private static entryReplacer(key: string, value: any): any {
        if (key === "Parent") { return undefined; }
        return value;
    }

    private static pathTo(moduleName: string, path: string = ""): string {
        return MODULE_PATH.replace("{moduleName}", moduleName) + path;
    }
}
