import DatabaseConfigModel from "../../models/DatabaseConfigModel";

export default class RestoreDatabaseRequest {
    Config: DatabaseConfigModel;
    BackupFileName: string;
}