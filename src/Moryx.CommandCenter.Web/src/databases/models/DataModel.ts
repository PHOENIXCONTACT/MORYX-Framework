/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import BackupModel from "./BackupModel";
import DatabaseConfigModel from "./DatabaseConfigModel";
import DbMigrationsModel from "./DbMigrationsModel";
import SetupModel from "./SetupModel";

export default class DataModel {
    public targetModel: string;
    public config: DatabaseConfigModel;
    public setups: SetupModel[];
    public backups: BackupModel[];
    public availableMigrations: DbMigrationsModel[];
    public appliedMigrations: DbMigrationsModel[];
}
