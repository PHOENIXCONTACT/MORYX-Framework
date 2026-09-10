/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Container from "@mui/material/Container";
import * as React from "react";
import { createPortal } from "react-dom";
import { connect } from "react-redux";
import { Navigate, Route, Routes } from "react-router";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import DatabasesRestClient from "../../databases/api/DatabasesRestClient";
import Databases from "../../databases/container/Databases";
import ModulesRestClient from "../../modules/api/ModulesRestClient";
import Modules from "../../modules/container/Modules";
import { ModuleServerModuleState } from "../../modules/models/ModuleServerModuleState";
import NotificationModel from "../../modules/models/NotificationModel";
import ServerModuleModel from "../../modules/models/ServerModuleModel";
import { updateHealthState, updateModules, updateNotifications } from "../../modules/redux/ModulesActions";
import CommonRestClient from "../api/CommonRestClient";
import { AppState } from "../redux/AppState";
import { updateIsConnected } from "../redux/CommonActions";
import { ActionType } from "../redux/Types";

interface AppPropModel {
  ModulesRestClient: ModulesRestClient;
  CommonRestClient: CommonRestClient;
  DatabasesRestClient: DatabasesRestClient;
  IsConnected: boolean;
  ShowWaitDialog: boolean;
  Modules: ServerModuleModel[];
}

interface AppDispatchPropModel {
  onUpdateModules?(modules: ServerModuleModel[]): void;
  onUpdateModuleHealthState?(moduleName: string, healthState: ModuleServerModuleState): void;
  onUpdateModuleNotifications?(moduleName: string, notifications: NotificationModel[]): void;
  onUpdateIsConnected?(isConnected: boolean): void;
}

const mapStateToProps = (state: AppState): AppPropModel => {
  return {
    ModulesRestClient: state.Modules.RestClient,
    CommonRestClient: state.Common.RestClient,
    DatabasesRestClient: state.Databases.RestClient,
    IsConnected: state.Common.IsConnected,
    ShowWaitDialog: state.Common.ShowWaitDialog,
    Modules: state.Modules.Modules,
  };
};

const mapDispatchToProps = (dispatch: React.Dispatch<ActionType<{}>>): AppDispatchPropModel => {
  return {
    onUpdateModules: (modules: ServerModuleModel[]) => dispatch(updateModules(modules)),
    onUpdateModuleHealthState: (moduleName: string, healthState: ModuleServerModuleState) =>
      dispatch(updateHealthState(moduleName, healthState)),
    onUpdateModuleNotifications: (moduleName: string, notifications: NotificationModel[]) =>
      dispatch(updateNotifications(moduleName, notifications)),
    onUpdateIsConnected: (isConnected: boolean) => dispatch(updateIsConnected(isConnected))
  };
};

function App(props: AppPropModel & AppDispatchPropModel) {
  const updateLoadAndModulesTimerRef = React.useRef<ReturnType<typeof setInterval> | null>(null);

  React.useEffect(() => {
    updateLoadAndModulesTimerRef.current = setInterval(loadAndModulesUpdater, 5000);
    props.ModulesRestClient.modules().then((data) => props.onUpdateModules?.(data));

    return () => {
      clearInterval(updateLoadAndModulesTimerRef.current ?? undefined);
    };
  }, []);

  const loadAndModulesUpdater = (): void => {
    props.Modules.forEach((module) => {
      props.ModulesRestClient.healthState(module.name).then((data) =>
        props.onUpdateModuleHealthState?.(module.name, data)
      );
      props.ModulesRestClient.notifications(module.name).then((data) =>
        props.onUpdateModuleNotifications?.(module.name, data)
      );
    });
  };

  return (
    <div>
      <div>
        {createPortal(<ToastContainer />, document.body)}

        <Container id="body">
          <Routes>
            <Route path="/modules/*" element={<Modules />} />
            <Route path="/databases/*" element={<Databases />} />
            <Route path="*" element={<Navigate to="/modules/*" />} />
          </Routes>
        </Container>
      </div>
    </div>
  );
}

export default connect(mapStateToProps, mapDispatchToProps)(App);
