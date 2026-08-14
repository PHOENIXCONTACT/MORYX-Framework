/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ActionType } from "../../common/redux/Types";
import ModulesRestClient from "../api/ModulesRestClient";
import Config from "../models/Config";
import { FailureBehaviour } from "../models/FailureBehaviour";
import { ModuleServerModuleState } from "../models/ModuleServerModuleState";
import { ModuleStartBehaviour } from "../models/ModuleStartBehaviour";
import NotificationModel from "../models/NotificationModel";
import ServerModuleModel from "../models/ServerModuleModel";
import { UPDATE_FAILURE_BEHAVIOUR, UPDATE_HEALTHSTATE, UPDATE_MODULES, UPDATE_NOTIFICATIONS, UPDATE_START_BEHAVIOUR } from "./ModulesActions";

export interface ModulesState {
  RestClient: ModulesRestClient;
  Modules: ServerModuleModel[];
  Configs: Config[];
}

export const initialModulesState: ModulesState = {
  Configs: [],
  Modules: [],
  RestClient: new ModulesRestClient(BASE_URL),
};

function updateModule(state: ModulesState, moduleName: string, update: Partial<ServerModuleModel>): ModulesState {
  return {
    ...state,
    Modules: state.Modules.map((module) =>
      module.name === moduleName ? { ...module, ...update } : module
    ),
  };
}

export function getModulesReducer(state: ModulesState = initialModulesState, action: ActionType<{}>): ModulesState {
  switch (action.type) {
    case UPDATE_MODULES: {
      return { ...state, Modules: action.payload as ServerModuleModel[] };
    }
    case UPDATE_HEALTHSTATE: {
      const { moduleName, healthState } = action.payload as { moduleName: string; healthState: ModuleServerModuleState };
      return updateModule(state, moduleName, { healthState });
    }
    case UPDATE_NOTIFICATIONS: {
      const { moduleName, notifications } = action.payload as { moduleName: string; notifications: NotificationModel[] };
      const sorted = notifications.sort((a, b) =>
        new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
      );
      return updateModule(state, moduleName, { notifications: sorted });
    }
    case UPDATE_START_BEHAVIOUR: {
      const { moduleName, startBehaviour } = action.payload as { moduleName: string; startBehaviour: ModuleStartBehaviour };
      return updateModule(state, moduleName, { startBehaviour });
    }
    case UPDATE_FAILURE_BEHAVIOUR: {
      const { moduleName, failureBehaviour } = action.payload as { moduleName: string; failureBehaviour: FailureBehaviour };
      return updateModule(state, moduleName, { failureBehaviour });
    }
  }
  return state;
}
