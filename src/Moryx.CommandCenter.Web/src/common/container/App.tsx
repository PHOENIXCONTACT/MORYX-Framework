/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { connect } from "react-redux";
import { Navigate, Route, Routes } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { Container } from "reactstrap";
import DatabasesRestClient from "../../databases/api/DatabasesRestClient";
import Databases from "../../databases/container/Databases";
import ModulesRestClient from "../../modules/api/ModulesRestClient";
import Modules from "../../modules/container/Modules";
import { ModuleServerModuleState } from "../../modules/models/ModuleServerModuleState";
import NotificationModel from "../../modules/models/NotificationModel";
import ServerModuleModel from "../../modules/models/ServerModuleModel";
import { updateHealthState, updateModules, updateNotifications } from "../../modules/redux/ModulesActions";
import CommonRestClient from "../api/CommonRestClient";
import ApplicationInformationResponse from "../api/responses/ApplicationInformationResponse";
import ApplicationLoadResponse from "../api/responses/ApplicationLoadResponse";
import HostInformationResponse from "../api/responses/HostInformationResponse";
import SystemLoadResponse from "../api/responses/SystemLoadResponse";
import { AppState } from "../redux/AppState";
import { updateIsConnected, updateServerTime } from "../redux/CommonActions";
import { ActionType } from "../redux/Types";
import "../scss/commandcenter.scss";

interface AppPropModel {
  ModulesRestClient: ModulesRestClient;
  CommonRestClient: CommonRestClient;
  DatabasesRestClient: DatabasesRestClient;
  IsConnected: boolean;
  ShowWaitDialog: boolean;
  Modules: ServerModuleModel[];
}

interface AppDispatchPropModel {
  onUpdateServerTime?(serverTime: string): void;
  onUpdateApplicationInfo?(applicationInfo: ApplicationInformationResponse): void;
  onUpdateHostInfo?(hostInfo: HostInformationResponse): void;
  onUpdateApplicationLoad?(applicationLoad: ApplicationLoadResponse): void;
  onUpdateSystemLoad?(systemLoad: SystemLoadResponse): void;
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
    onUpdateServerTime: (serverTime: string) => dispatch(updateServerTime(serverTime)),
    onUpdateModules: (modules: ServerModuleModel[]) => dispatch(updateModules(modules)),
    onUpdateModuleHealthState: (moduleName: string, healthState: ModuleServerModuleState) =>
      dispatch(updateHealthState(moduleName, healthState)),
    onUpdateModuleNotifications: (moduleName: string, notifications: NotificationModel[]) =>
      dispatch(updateNotifications(moduleName, notifications)),
    onUpdateIsConnected: (isConnected: boolean) => dispatch(updateIsConnected(isConnected))
  };
};

function App(props: AppPropModel & AppDispatchPropModel) {
  const updateClockTimerRef = React.useRef<NodeJS.Timeout>();
  const updateLoadAndModulesTimerRef = React.useRef<NodeJS.Timeout>();

  React.useEffect(() => {
    updateLoadAndModulesTimerRef.current = setInterval(loadAndModulesUpdater, 5000);
    props.ModulesRestClient.modules().then((data) => props?.onUpdateModules(data));

    return () => {
      clearInterval(updateClockTimerRef.current!);
      clearInterval(updateLoadAndModulesTimerRef.current!);
    };
  }, []);

  const loadAndModulesUpdater = (): void => {
    props.Modules.forEach((module) => {
      props.ModulesRestClient.healthState(module.name).then((data) =>
        props.onUpdateModuleHealthState(module.name, data)
      );
      props.ModulesRestClient.notifications(module.name).then((data) =>
        props.onUpdateModuleNotifications(module.name, data)
      );
    });
  };

  return (
    <div className="commandcenter-app-container">
      <div className="commandcenter-content-wrapper">
        <ToastContainer />

        <Container fluid={true} id="body" className="content">
          <Routes>
            <Route path="/modules/*" element={<Modules />} />
            <Route path="/databases/*" element={<Databases />} />
            <Route path="*" element={<Navigate to="/databases/*" />} />
          </Routes>
        </Container>
      </div>
    </div>
  );
}

export default connect(mapStateToProps, mapDispatchToProps)(App);
