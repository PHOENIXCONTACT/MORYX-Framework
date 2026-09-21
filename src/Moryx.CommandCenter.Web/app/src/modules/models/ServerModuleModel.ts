/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import AssemblyModel from "./AssemblyModel";
import { ModuleServerModuleState } from "./ModuleServerModuleState";
import { ModuleStartBehaviour } from "./ModuleStartBehaviour";
import NotificationModel from "./NotificationModel";

export default class ServerModuleModel {
    public name: string;
    public healthState: ModuleServerModuleState;
    public startBehaviour: ModuleStartBehaviour;
    public dependencies: ServerModuleModel[];
    public notifications: NotificationModel[];
    public assembly: AssemblyModel;
}
