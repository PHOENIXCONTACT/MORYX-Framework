/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ModuleServerModuleState } from './ModuleServerModuleState';
import { ModuleStartBehaviour } from './ModuleStartBehaviour';
import { FailureBehaviour } from './FailureBehaviour';
import NotificationModel from './NotificationModel';
import AssemblyModel from './AssemblyModel';
import MethodEntry from './MethodEntry';

export default class ServerModuleModel
{
    name : string;
    healthState : ModuleServerModuleState;
    startBehaviour : ModuleStartBehaviour;
    failureBehaviour : FailureBehaviour;
    dependencies : ServerModuleModel[];
    notifications : NotificationModel[];
    assembly : AssemblyModel;
}
