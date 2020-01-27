import ApplicationInformationResponse from "./responses/ApplicationInformationResponse";
import ApplicationLoadResponse from "./responses/ApplicationLoadResponse";
import HostInformationResponse from "./responses/HostInformationResponse";
import ServerTimeResponse from "./responses/ServerTimeResponse";
import SuccessResponse from "./responses/SuccessResponse";
import SystemLoadResponse from "./responses/SystemLoadResponse";
import RestClientBase from "./RestClientBase";

export default class CommonRestClient extends RestClientBase {
    public serverTime(): Promise<ServerTimeResponse> {
        return this.get<ServerTimeResponse>("/CommonMaintenance/ServerTime", new ServerTimeResponse());
    }

    public applicationInfo(): Promise<ApplicationInformationResponse> {
        return this.get<ApplicationInformationResponse>("/CommonMaintenance/ApplicationInfo", new ApplicationInformationResponse());
    }

    public hostInfo(): Promise<HostInformationResponse> {
        return this.get<HostInformationResponse>("/CommonMaintenance/HostInfo", new HostInformationResponse());
    }

    public systemLoad(): Promise<SystemLoadResponse> {
        return this.get<SystemLoadResponse>("/CommonMaintenance/SystemLoad", new SystemLoadResponse());
    }

    public applicationLoad(): Promise<ApplicationLoadResponse> {
        return this.get<ApplicationLoadResponse>("/CommonMaintenance/ApplicationLoad", new ApplicationLoadResponse());
    }
}
