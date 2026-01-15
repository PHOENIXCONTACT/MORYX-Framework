/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

export enum TestConnectionResult {
    ConfigurationError = "ConfigurationError",
    ConnectionError = "ConnectionError",
    ConnectionOkDbDoesNotExist = "ConnectionOkDbDoesNotExist",
    Success = "Success",
    PendingMigrations = "PendingMigrations",
}
