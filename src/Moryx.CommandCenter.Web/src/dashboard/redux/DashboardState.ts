/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import ApplicationInformationResponse from "../../common/api/responses/ApplicationInformationResponse";
import ApplicationLoadResponse from "../../common/api/responses/ApplicationLoadResponse";
import HostInformationResponse from "../../common/api/responses/HostInformationResponse";
import SystemLoadResponse from "../../common/api/responses/SystemLoadResponse";
import SystemLoadSample from "../../common/models/SystemLoadSample";
import { ActionType } from "../../common/redux/Types";
import { UPDATE_APPLICATION_INFO, UPDATE_APPLICATION_LOAD, UPDATE_HOST_INFO, UPDATE_SYSTEM_LOAD } from "./DashboardActions";

export interface DashboardState {
    ApplicationInfo: ApplicationInformationResponse;
    HostInfo: HostInformationResponse;
    ApplicationLoad: ApplicationLoadResponse;
    SystemLoad: SystemLoadSample[];
}

export const initialDashboardState: DashboardState = {
    ApplicationInfo: { assemblyProduct: "", assemblyVersion: "", assemblyInformationalVersion: "", assemblyDescription: "" },
    HostInfo: { machineName: "", osInformation: "", upTime: 0 },
    ApplicationLoad: { cpuLoad: 0, systemMemory: 0, workingSet: 0 },
    SystemLoad: [],
};

export function getDashboardReducer(state: DashboardState = initialDashboardState, action: ActionType<{}>): DashboardState {
  switch (action.type) {
    case UPDATE_APPLICATION_INFO: {
        return { ...state, ApplicationInfo: action.payload as ApplicationInformationResponse };
    }
    case UPDATE_HOST_INFO: {
        return { ...state, HostInfo: action.payload as HostInformationResponse };
    }
    case UPDATE_APPLICATION_LOAD: {
        return { ...state, ApplicationLoad: action.payload as ApplicationLoadResponse };
    }
    case UPDATE_SYSTEM_LOAD: {
        const systemLoadResponse = action.payload as SystemLoadResponse;
        const samples = state.SystemLoad.slice();
        if (samples.length === 5) {
            samples.shift();
        }

        samples.push({ Date: new Date(Date.now()).toLocaleString(), CPULoad: systemLoadResponse.cpuLoad, SystemMemoryLoad: systemLoadResponse.systemMemoryLoad });

        return { ...state, SystemLoad: samples };
    }
  }
  return state;
}
