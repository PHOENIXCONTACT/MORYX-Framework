/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { MigrationResult } from "./MigrationResult";

export default class DatabaseMigrationSummary {
    public result: MigrationResult;
    public executedMigrations: string[];
    public errors: string[];
}

