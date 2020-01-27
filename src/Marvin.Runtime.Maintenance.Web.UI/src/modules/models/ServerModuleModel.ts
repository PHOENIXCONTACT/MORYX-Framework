import { ModuleServerModuleState } from './ModuleServerModuleState';
import { ModuleStartBehaviour } from './ModuleStartBehaviour';
import { FailureBehaviour } from './FailureBehaviour';
import NotificationModel from './NotificationModel';
import AssemblyModel from './AssemblyModel';
import MethodEntry from './MethodEntry';

export default class ServerModuleModel
{
    Name : string;
    HealthState : ModuleServerModuleState;
    StartBehaviour : ModuleStartBehaviour;
    FailureBehaviour : FailureBehaviour;
    Dependencies : ServerModuleModel[];
    Notifications : NotificationModel[];
    Assembly : AssemblyModel;
}