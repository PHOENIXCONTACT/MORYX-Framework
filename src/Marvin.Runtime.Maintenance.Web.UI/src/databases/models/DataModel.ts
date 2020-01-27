import BackupModel from "./BackupModel";
import DatabaseConfigModel from "./DatabaseConfigModel";
import DbMigrationsModel from "./DbMigrationsModel";
import ScriptModel from "./ScriptModel";
import SetupModel from "./SetupModel";

export default class DataModel {
    public TargetModel: string;
    public Config: DatabaseConfigModel;
    public Setups: SetupModel[];
    public Backups: BackupModel[];
    public Scripts: ScriptModel[];
    public AvailableMigrations: DbMigrationsModel[];
    public AppliedMigrations: DbMigrationsModel[];
}
