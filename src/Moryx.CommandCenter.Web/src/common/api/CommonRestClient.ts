/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import ApplicationInformationResponse from "./responses/ApplicationInformationResponse";
import ApplicationLoadResponse from "./responses/ApplicationLoadResponse";
import HostInformationResponse from "./responses/HostInformationResponse";
import ServerTimeResponse from "./responses/ServerTimeResponse";
import SystemLoadResponse from "./responses/SystemLoadResponse";
import RestClientBase from "./RestClientBase";

const ROOT_PATH = "/common";
const INFO_PATH = ROOT_PATH + "/info";

export default class CommonRestClient extends RestClientBase {

    public serverTime(): Promise<ServerTimeResponse> {
        return this.get<ServerTimeResponse>(ROOT_PATH + "/time", new ServerTimeResponse());
    }

    public applicationInfo(): Promise<ApplicationInformationResponse> {
        return this.get<ApplicationInformationResponse>(INFO_PATH + "/application", new ApplicationInformationResponse());
    }

    public applicationLoad(): Promise<ApplicationLoadResponse> {
        return this.get<ApplicationLoadResponse>(INFO_PATH + "/application/load", new ApplicationLoadResponse());
    }

    public hostInfo(): Promise<HostInformationResponse> {
        return this.get<HostInformationResponse>(INFO_PATH + "/system", new HostInformationResponse());
    }

    public systemLoad(): Promise<SystemLoadResponse> {
        return this.get<SystemLoadResponse>(INFO_PATH + "/system/load", new SystemLoadResponse());
    }
}
