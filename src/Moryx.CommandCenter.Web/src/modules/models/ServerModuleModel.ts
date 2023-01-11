/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import AssemblyModel from "./AssemblyModel";
import { FailureBehaviour } from "./FailureBehaviour";
import { ModuleServerModuleState } from "./ModuleServerModuleState";
import { ModuleStartBehaviour } from "./ModuleStartBehaviour";
import NotificationModel from "./NotificationModel";

export default class ServerModuleModel {
    public name: string;
    public healthState: ModuleServerModuleState;
    public startBehaviour: ModuleStartBehaviour;
    public failureBehaviour: FailureBehaviour;
    public dependencies: ServerModuleModel[];
    public notifications: NotificationModel[];
    public assembly: AssemblyModel;
}
