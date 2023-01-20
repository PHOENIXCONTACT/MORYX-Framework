/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Entry from "../../modules/models/Entry";
import BackupModel from "./BackupModel";
import DbMigrationsModel from "./DbMigrationsModel";
import SetupModel from "./SetupModel";

export default class DataModel {
    public targetModel: string;
    public config: Entry;
    public setups: SetupModel[];
    public backups: BackupModel[];
    public availableMigrations: DbMigrationsModel[];
    public appliedMigrations: DbMigrationsModel[];
}
