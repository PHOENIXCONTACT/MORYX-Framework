import { LogLevel } from "../../models/LogLevel";

export default class AddRemoteAppenderRequest {
    public Name: string;
    public MinLevel: LogLevel;
}
