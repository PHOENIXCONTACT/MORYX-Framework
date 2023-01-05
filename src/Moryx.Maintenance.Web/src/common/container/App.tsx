/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import NotificationSystem = require("react-notification-system");
import { connect } from "react-redux";
import { Redirect, Route, RouteComponentProps, Switch, withRouter } from "react-router-dom";
import { Container, Row } from "reactstrap";
import DatabasesRestClient from "../../databases/api/DatabasesRestClient";
import Databases from "../../databases/container/Databases";
import LogRestClient from "../../log/api/LogRestClient";
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
import { updateIsConnected, updateNotificationInstance, updateServerTime } from "../redux/CommonActions";
import { ActionType } from "../redux/Types";
import "../scss/maintenance.scss";

interface AppPropModel {
    ModulesRestClient: ModulesRestClient;
    CommonRestClient: CommonRestClient;
    DatabasesRestClient: DatabasesRestClient;
    LogRestClient: LogRestClient;
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
    onUpdateNotificationSystemInstance?(notificationSystem: NotificationSystem): void;
}

const mapStateToProps = (state: AppState): AppPropModel => {
    return {
        ModulesRestClient: state.Modules.RestClient,
        CommonRestClient: state.Common.RestClient,
        DatabasesRestClient: state.Databases.RestClient,
        LogRestClient: state.Log.RestClient,
        IsConnected: state.Common.IsConnected,
        ShowWaitDialog: state.Common.ShowWaitDialog,
        Modules: state.Modules.Modules,
    };
};

const mapDispatchToProps = (dispatch: React.Dispatch<ActionType<{}>>): AppDispatchPropModel => {
    return {
        onUpdateServerTime: (serverTime: string) => dispatch(updateServerTime(serverTime)),
        onUpdateModules: (modules: ServerModuleModel[]) => dispatch(updateModules(modules)),
        onUpdateModuleHealthState: (moduleName: string, healthState: ModuleServerModuleState) => dispatch(updateHealthState(moduleName, healthState)),
        onUpdateModuleNotifications: (moduleName: string, notifications: NotificationModel[]) => dispatch(updateNotifications(moduleName, notifications)),
        onUpdateIsConnected: (isConnected: boolean) => dispatch(updateIsConnected(isConnected)),
        onUpdateNotificationSystemInstance: (notificationSystem: NotificationSystem) => dispatch(updateNotificationInstance(notificationSystem)),
    };
};

class App extends React.Component<AppPropModel & RouteComponentProps<{}> & AppDispatchPropModel> {
    private updateClockTimer: NodeJS.Timeout;
    private updateLoadAndModulesTimer: NodeJS.Timeout;
    private notificationSystem: NotificationSystem = null;

    constructor(props: AppPropModel & RouteComponentProps<{}> & AppDispatchPropModel) {
        super(props);

        this.loadAndModulesUpdater = this.loadAndModulesUpdater.bind(this);
    }

    public componentDidMount(): void {
        this.updateLoadAndModulesTimer = setInterval(this.loadAndModulesUpdater, 5000);

        this.props.CommonRestClient.applicationInfo().then((data) => this.props.onUpdateApplicationInfo(data));
        this.props.CommonRestClient.hostInfo().then((data) => this.props.onUpdateHostInfo(data));
        this.props.CommonRestClient.applicationLoad().then((data) => this.props.onUpdateApplicationLoad(data));
        this.props.CommonRestClient.systemLoad().then((data) => this.props.onUpdateSystemLoad(data));
        this.props.ModulesRestClient.modules().then((data) => this.props.onUpdateModules(data));
    }

    public componentWillUnmount(): void {
        clearInterval(this.updateClockTimer);
        clearInterval(this.updateLoadAndModulesTimer);
    }

    public render(): React.ReactNode {
        const ref = (instance: NotificationSystem) => {
            if (this.notificationSystem == null) {
                this.notificationSystem = instance;
                this.props.onUpdateNotificationSystemInstance(instance);
            }
        };

        return (
            <div className="maintenance-app-container">
                <div className="maintenance-content-wrapper">
                    <NotificationSystem ref={ref}/>

                    <Container fluid={true} id="body" className="content">
                        <Switch>
                            <Route path="/modules" component={Modules} />
                            <Route path="/databases" component={Databases} />
                            <Redirect to="/databases" />
                        </Switch>
                    </Container>
                </div>
                <footer>
                    <Container fluid={true}>
                        <Row><hr /></Row>
                        <Row>
                            <ul>
                                <li>&copy; 2022 PHOENIX CONTACT</li>
                            </ul>
                        </Row>
                    </Container>
                </footer>
            </div>
        );
    }

    private loadAndModulesUpdater(): void {
        this.props.CommonRestClient.systemLoad().then((data) => this.props.onUpdateSystemLoad(data));
        this.props.Modules.forEach((module) => {
            this.props.ModulesRestClient.healthState(module.name).then((data) => this.props.onUpdateModuleHealthState(module.name, data));
            this.props.ModulesRestClient.notifications(module.name).then((data) => this.props.onUpdateModuleNotifications(module.name, data));
        });
    }
}

export default withRouter<RouteComponentProps<{}>, React.ComponentType<any>>(connect<AppPropModel, AppDispatchPropModel>(mapStateToProps, mapDispatchToProps)(App));
