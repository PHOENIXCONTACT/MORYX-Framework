/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

export enum ModuleServerModuleState {
    Stopped =  0x0,
    Initializing = 0x2,
    Ready = 0x1,
    Starting = 0x3,
    Running = 0x8,
    Stopping = 0xA,
    Failure = 0x4,
}
