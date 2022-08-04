/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import BackupModel from "./BackupModel";
import DatabaseConfigModel from "./DatabaseConfigModel";
import DbMigrationsModel from "./DbMigrationsModel";
import SetupModel from "./SetupModel";

export default class DataModel {
    public TargetModel: string;
    public Config: DatabaseConfigModel;
    public Setups: SetupModel[];
    public Backups: BackupModel[];
    public AvailableMigrations: DbMigrationsModel[];
    public AppliedMigrations: DbMigrationsModel[];
}
