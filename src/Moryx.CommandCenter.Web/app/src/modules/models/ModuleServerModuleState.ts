/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

export enum ModuleServerModuleState {
    Stopped = "Stopped",
    Initializing = "Initializing",
    Ready = "Ready",
    Starting = "Starting",
    Running = "Running",
    Stopping = "Stopping",
    Failure = "Failure",
    Missing = "Missing",
}
