/*
 * Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import DatabaseConfigModel from "./DatabaseConfigModel";
import DbMigrationsModel from "./DbMigrationsModel";
import SetupModel from "./SetupModel";

export default class DataModel {
    public targetModel: string;
    public config: DatabaseConfigModel;
    public setups: SetupModel[];
    public availableMigrations: DbMigrationsModel[];
    public appliedMigrations: DbMigrationsModel[];
    public possibleConfigurators: string[];
}
