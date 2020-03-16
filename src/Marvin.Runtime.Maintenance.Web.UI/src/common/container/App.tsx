/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { faBars, faCubes, faDatabase, faSitemap } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import * as moment from "moment";
import * as React from "react";
import NotificationSystem = require("react-notification-system");
import { connect } from "react-redux";
import { Link, Route, RouteComponentProps, Switch, withRouter } from "react-router-dom";
import { Card, CardBody, CardFooter, CardHeader, CardText, CardTitle, Col, Collapse, Container, Modal, ModalBody, ModalHeader, Nav, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink, Row } from "reactstrap";
import Dashboard from "../../dashboard/container/Dashboard";
import { updateApplicationInfo, updateApplicationLoad, updateHostInfo, updateSystemLoad } from "../../dashboard/redux/DashboardActions";
import Databases from "../../databases/container/Databases";
import DataModel from "../../databases/models/DataModel";
import Log from "../../log/container/Log";
import ModulesRestClient from "../../modules/api/ModulesRestClient";
import Modules from "../../modules/container/Modules";
import Config from "../../modules/models/Config";
import { ModuleServerModuleState } from "../../modules/models/ModuleServerModuleState";
import NotificationModel from "../../modules/models/NotificationModel";
import ServerModuleModel from "../../modules/models/ServerModuleModel";
import { updateHealthState, updateModules, updateNotifications } from "../../modules/redux/ModulesActions";
import { VERSION } from "../../Version";
import CommonRestClient from "../api/CommonRestClient";
import ApplicationInformationResponse from "../api/responses/ApplicationInformationResponse";
import ApplicationLoadResponse from "../api/responses/ApplicationLoadResponse";
import HostInformationResponse from "../api/responses/HostInformationResponse";
import SystemLoadResponse from "../api/responses/SystemLoadResponse";
import Clock from "../components/Clock";
import RestClientEndpoint from "../models/RestClientEnpoint";
import { AppState } from "../redux/AppState";
import { updateIsConnected, updateNotificationInstance, updateRestClientEndpoint, updateServerTime } from "../redux/CommonActions";
import { ActionType } from "../redux/Types";
import "../scss/maintenance.scss";

interface AppPropModel {
    ModulesRestClient: ModulesRestClient;
    RestClient: CommonRestClient;
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
    onUpdateNotificationSystemInstance?(notificationSystem: NotificationSystem.System): void;
}

const mapStateToProps = (state: AppState): AppPropModel => {
    return {
        ModulesRestClient: state.Modules.RestClient,
        RestClient: state.Common.RestClient,
        IsConnected: state.Common.IsConnected,
        ShowWaitDialog: state.Common.ShowWaitDialog,
        Modules: state.Modules.Modules,
    };
};

const mapDispatchToProps = (dispatch: React.Dispatch<ActionType<{}>>): AppDispatchPropModel => {
    return {
        onUpdateServerTime: (serverTime: string) => dispatch(updateServerTime(serverTime)),
        onUpdateApplicationInfo: (applicationInfo: ApplicationInformationResponse) => dispatch(updateApplicationInfo(applicationInfo)),
        onUpdateHostInfo: (hostInfo: HostInformationResponse) => dispatch(updateHostInfo(hostInfo)),
        onUpdateApplicationLoad: (applicationLoad: ApplicationLoadResponse) => dispatch(updateApplicationLoad(applicationLoad)),
        onUpdateSystemLoad: (systemLoad: SystemLoadResponse) => dispatch(updateSystemLoad(systemLoad)),
        onUpdateModules: (modules: ServerModuleModel[]) => dispatch(updateModules(modules)),
        onUpdateModuleHealthState: (moduleName: string, healthState: ModuleServerModuleState) => dispatch(updateHealthState(moduleName, healthState)),
        onUpdateModuleNotifications: (moduleName: string, notifications: NotificationModel[]) => dispatch(updateNotifications(moduleName, notifications)),
        onUpdateIsConnected: (isConnected: boolean) => dispatch(updateIsConnected(isConnected)),
        onUpdateNotificationSystemInstance: (notificationSystem: NotificationSystem.System)  => dispatch(updateNotificationInstance(notificationSystem)),
    };
};

class App extends React.Component<AppPropModel & RouteComponentProps<{}> & AppDispatchPropModel> {
    private updateClockTimer: NodeJS.Timeout;
    private updateLoadAndModulesTimer: NodeJS.Timeout;
    private notificationSystem: NotificationSystem.System = null;

    constructor(props: AppPropModel & RouteComponentProps<{}> & AppDispatchPropModel) {
        super(props);

        this.clockUpdater = this.clockUpdater.bind(this);
        this.loadAndModulesUpdater = this.loadAndModulesUpdater.bind(this);
    }

    public componentDidMount(): void {
        this.updateClockTimer = setInterval(this.clockUpdater, 1000);
        this.updateLoadAndModulesTimer = setInterval(this.loadAndModulesUpdater, 5000);

        this.props.RestClient.applicationInfo().then((data) => this.props.onUpdateApplicationInfo(data));
        this.props.RestClient.hostInfo().then((data) => this.props.onUpdateHostInfo(data));
        this.props.RestClient.applicationLoad().then((data) => this.props.onUpdateApplicationLoad(data));
        this.props.RestClient.systemLoad().then((data) => this.props.onUpdateSystemLoad(data));
        this.props.ModulesRestClient.modules().then((data) => this.props.onUpdateModules(data));
    }

    public componentWillUnmount(): void {
        clearInterval(this.updateClockTimer);
        clearInterval(this.updateLoadAndModulesTimer);
    }

    public render(): React.ReactNode {
        const ref = (instance: NotificationSystem.System) => {
            if (this.notificationSystem == null) {
                this.notificationSystem = instance;
                this.props.onUpdateNotificationSystemInstance(instance);
            }
        };

        return (
            <div>
                <div className="main-content">
                    <NotificationSystem ref={ref}/>
                    <Row className="header hidden-xs hidden-sm">
                        <Col md={2}>
                            <svg id="pxclogo">
                                <path d="M88.978,19.7C88.978,19.7 87.942,23.025 87.942,23.025 87.942,23.025 90.016,23.025 90.016,23.025 90.016,23.025 88.978,19.7 88.978,19.7z M48.256,18.957C46.795,18.964 45.944,20.456 45.961,21.916 45.978,23.355 46.819,24.8 48.259,24.804 49.702,24.809 50.555,
                                        23.36 50.572,21.916 50.59,20.451 49.722,18.95 48.256,18.957z M71.916,15.172C71.916,15.172 83.393,15.172 83.393,15.172 83.393,15.172 83.393,18.721 83.393,18.721 83.393,18.721 80.104,18.723 80.104,18.723 80.104,18.723 80.104,28.531 80.104,28.531 80.104,
                                        28.531 75.296,28.531 75.296,28.531 75.296,28.531 75.297,18.722 75.297,18.722 75.297,18.722 71.916,18.725 71.916,18.725 71.916,18.725 71.916,15.172 71.916,15.172z M116.259,15.171C116.259,15.171 116.259,18.722 116.259,18.722 116.259,18.722 112.938,
                                        18.722 112.938,18.722 112.938,18.722 112.951,28.531 112.951,28.531 112.951,28.531 108.117,28.531 108.117,28.531 108.117,28.531 108.117,18.722 108.117,18.722 108.117,18.722 103.772,18.724 103.772,18.724 101.783,18.724 100.881,20.246 100.881,
                                        21.892 100.88,23.737 102.432,25.076 104.267,25.165 105.3,25.215 105.992,24.977 106.775,24.302 106.775,24.302 106.775,27.922 106.775,27.922 105.569,28.586 104.736,28.804 103.359,28.816 99.443,28.85 96.071,25.825 96.084,21.892 96.096,18.063 98.302,
                                        15.173 103.37,15.172 107.595,15.172 116.259,15.171 116.259,15.171z M86.401,15.171C86.401,15.171 91.573,15.171 91.573,15.171 91.573,15.171 97.031,28.531 97.031,28.531 97.031,28.531 92.091,28.531 92.091,28.531 92.091,28.531 91.14,26.115 91.14,26.115 91.14,
                                        26.115 86.832,26.115 86.832,26.115 86.832,26.115 85.884,28.532 85.884,28.532 85.884,28.532 80.883,28.531 80.883,28.531 80.883,28.531 86.401,15.171 86.401,15.171z M66.358,15.171C66.358,15.171 71.289,15.173 71.289,15.173 71.289,15.173 71.289,28.531 71.289,
                                        28.531 71.289,28.531 67.237,28.531 67.237,28.531 67.237,28.531 61.603,22.875 61.603,22.875 61.603,22.875 61.605,28.531 61.605,28.531 61.605,28.531 56.751,28.531 56.751,28.531 56.751,28.531 56.751,15.173 56.751,15.173 56.751,15.173 60.585,15.173 60.585,
                                        15.173 60.585,15.173 66.356,20.929 66.356,20.929 66.356,20.929 66.358,15.171 66.358,15.171z M48.228,14.904C52.672,14.895 55.501,17.964 55.482,21.892 55.463,25.802 52.672,28.81 48.227,28.813 43.916,28.816 41.093,25.773 41.075,21.892 41.057,17.992 43.916,
                                        14.912 48.228,14.904z M36.159,14.887C37.72,14.889 38.852,14.996 40.105,15.571 40.101,15.523 40.105,19.537 40.105,19.537 39.37,18.955 38.422,18.608 37.323,18.611 35.507,18.616 34.059,20.075 34.061,21.892 34.064,23.656 35.52,25.067 37.284,25.068 38.522,
                                        25.069 39.429,24.658 40.19,24.173 40.19,24.173 40.19,28.053 40.19,28.053 38.889,28.675 38.154,28.816 36.569,28.816 32.088,28.816 29.044,25.885 29.028,21.892 29.011,17.993 32.352,14.882 36.159,14.887z M13.712,10.414C12.646,10.414 11.781,11.278 11.781,
                                        12.344 11.781,13.41 12.646,14.275 13.712,14.275 14.778,14.275 15.642,13.41 15.642,12.344 15.642,11.278 14.778,10.414 13.712,10.414z M64.909,4.045C63.485,4.051 62.632,5.49 62.65,6.913 62.667,8.316 63.51,9.706 64.912,9.71 66.32,9.715 67.174,8.32 67.191,
                                        6.913 67.208,5.484 66.338,4.038 64.909,4.045z M4.69,3.4C3.884,3.4 3.223,4.063 3.22,4.869 3.22,4.869 3.213,23.884 3.213,23.884 3.211,24.691 3.983,25.377 4.789,25.377 4.789,25.377 11.186,25.377 11.186,25.377 11.992,25.377 12.659,24.717 12.662,23.91 12.662,
                                        23.91 12.66,17.629 12.66,17.629 12.66,16.342 11.65,15.769 11.65,15.769 10.702,15.088 9.683,13.877 9.696,12.337 9.708,11.051 10.155,10.221 11.107,9.355 11.801,8.723 12.666,8.529 12.666,7.538 12.666,7.538 12.66,4.952 12.66,4.952 12.66,4.13 12.66,
                                        3.394 11.206,3.4 11.206,3.4 4.69,3.4 4.69,3.4z M16.192,3.383C14.739,3.383 14.739,4.12 14.739,4.942 14.739,4.942 14.739,7.53 14.739,7.53 14.739,8.491 15.648,8.734 16.307,9.355 17.164,10.163 17.696,11.055 17.703,12.34 17.712,13.88 16.757,15.116 15.747,
                                        15.769 15.747,15.769 14.73,16.307 14.73,17.619 14.73,17.619 14.745,20.06 14.745,20.06 14.745,20.06 14.658,21.247 16.17,21.247 21.234,21.247 24.919,16.748 24.919,12.247 24.919,7.794 20.972,3.383 16.192,3.383z M35.422,3.372C35.422,3.372 34.659,3.373 34.659,
                                        3.373 34.659,3.373 34.658,6.439 34.658,6.439 34.658,6.439 35.422,6.439 35.422,6.439 36.458,6.439 37.152,5.793 37.143,4.89 37.134,3.995 36.458,3.372 35.422,3.372z M100.624,0.178C100.624,0.178 106.149,0.178 106.149,0.178 106.149,0.178 107.453,1.968 108.105,
                                        2.864L108.452,3.34 108.437,3.361C108.437,3.361,108.467,3.361,108.467,3.361L108.452,3.34 108.803,2.864C109.461,1.968 110.777,0.178 110.777,0.178 110.777,0.178 116.285,0.178 116.285,0.178 116.285,0.178 111.296,6.688 111.296,6.688 113.029,8.991 116.312,
                                        13.52 116.312,13.52 116.312,13.52 110.709,13.52 110.709,13.52 110.709,13.52 109.431,11.501 108.792,10.491L108.452,9.954 108.467,9.93C108.467,9.93,108.437,9.93,108.437,9.93L108.452,9.954 108.112,10.491C107.473,11.501 106.195,13.52 106.195,13.52 106.195,
                                        13.52 100.591,13.52 100.591,13.52 100.591,13.52 103.874,8.991 105.607,6.688 105.607,6.688 100.624,0.178 100.624,0.178z M95.012,0.178C95.155,0.178 95.358,0.178 95.601,0.178 97.064,0.178 99.988,0.178 99.988,0.178 99.988,0.178 99.988,13.52 99.988,
                                        13.52 99.988,13.52 94.789,13.52 94.789,13.52 94.789,13.52 94.789,0.178 94.789,0.178 94.789,0.178 94.87,0.178 95.012,0.178z M64.922,0.177C64.922,0.177 77.063,0.177 77.063,0.177 77.063,0.177 77.063,3.383 77.063,3.383 77.063,3.383 71.605,3.383 71.605,
                                        3.383 71.605,3.383 71.605,5.232 71.605,5.232 71.605,5.232 76.998,5.23 76.998,5.23 76.998,5.23 76.998,8.405 76.998,8.405 76.998,8.405 71.605,8.406 71.605,8.406 71.605,8.406 71.605,10.27 71.605,10.27 71.605,10.27 77.063,10.27 77.063,10.27 77.063,10.27 77.068,
                                        13.519 77.068,13.519 77.068,13.519 64.925,13.52 64.925,13.52 62.53,13.52 60.912,12.678 59.786,11.605 58.786,10.656 57.915,8.838 57.915,6.894 57.915,4.825 58.442,3.189 59.786,1.919 61.156,0.623 62.549,0.177 64.922,0.177z M29.786,0.177C29.786,0.177 36.458,
                                        0.177 36.458,0.177 40.082,0.177 41.975,2.056 41.995,4.892 42.013,7.744 39.216,9.671 36.458,9.671 36.458,9.671 34.657,9.671 34.657,9.671 34.657,9.671 34.656,13.52 34.656,13.52 34.656,13.52 29.8,13.52 29.8,13.52 29.8,13.52 29.786,0.177 29.786,0.177z M1.986,
                                        0.177C1.986,0.177 26.109,0.177 26.109,0.177 27.205,0.177 28.094,1.065 28.094,2.162 28.094,2.162 28.094,26.548 28.094,26.548 28.094,27.645 27.205,28.534 26.109,28.534 26.109,28.534 1.986,28.534 1.986,28.534 0.889,28.534 0,27.645 0,26.548 0,26.548 0,2.162 0,
                                        2.162 0,1.065 0.889,0.177 1.986,0.177z M78.507,0.177C78.507,0.177 82.349,0.177 82.349,0.177 82.349,0.177 88.313,5.833 88.313,5.833 88.313,5.833 88.313,0.177 88.313,0.177 88.313,0.177 93.148,0.177 93.148,0.177 93.148,0.177 93.148,13.52 93.148,13.52 93.148,
                                        13.52 89.077,13.52 89.077,13.52 89.077,13.52 83.342,7.881 83.342,7.881 83.342,7.881 83.342,13.52 83.342,13.52 83.342,13.52 78.5,13.52 78.5,13.52 78.5,13.52 78.507,0.177 78.507,0.177z M42.751,0.176C42.751,0.176 42.827,0.176 42.959,0.176 43.091,0.176 43.28,
                                        0.176 43.506,0.176 44.865,0.176 47.584,0.176 47.584,0.176 47.584,0.176 47.583,4.732 47.583,4.732 47.583,4.732 51.743,4.733 51.743,4.733 51.743,4.733 51.743,0.176 51.743,0.176 51.743,0.176 56.57,0.176 56.57,0.176 56.57,0.176 56.57,13.519 56.57,13.519 56.57,
                                        13.519 51.743,13.519 51.743,13.519 51.743,13.519 51.743,8.656 51.743,8.656 51.743,8.656 47.584,8.656 47.584,8.656 47.584,8.656 47.584,13.519 47.584,13.519 47.584,13.519 42.751,13.52 42.751,13.52 42.751,13.52 42.751,0.176 42.751,0.176z" />
                            </svg>
                        </Col>
                        <Col id="clock" md={{ size: 2, offset: 8 }} className="text-right">
                            <Clock />
                        </Col>
                    </Row>

                    <Navbar className="navbar-default" expand="md">
                        <NavbarBrand href="#/">
                            <FontAwesomeIcon icon={faCubes} className="right-space" />
                            Maintenance
                        </NavbarBrand>
                        <Collapse navbar={true}>
                            <Nav className="navbar-left" navbar={true}>
                                <NavItem className={this.isRouteActive("/modules") ? "active" : ""}>
                                    <Link to="/modules">
                                        <FontAwesomeIcon icon={faSitemap} className="right-space" />
                                        Modules
                                    </Link>
                                </NavItem>
                                <NavItem className={this.isRouteActive("/databases") ? "active" : ""}>
                                    <Link to="/databases">
                                        <FontAwesomeIcon icon={faDatabase} className="right-space" />
                                        Databases
                                    </Link>
                                </NavItem>
                                <NavItem className={this.isRouteActive("/log") ? "active" : ""}>
                                    <Link to="/log">
                                        <FontAwesomeIcon icon={faBars} className="right-space" />
                                        Log
                                    </Link>
                                </NavItem>
                            </Nav>
                        </Collapse>
                    </Navbar>

                    <Container fluid={true} id="body" className="content">
                        <Switch>
                            <Route exact={true} path="/" component={Dashboard} />
                            <Route path="/modules" component={Modules} />
                            <Route path="/databases" component={Databases} />
                            <Route path="/log" component={Log} />
                        </Switch>
                    </Container>

                    <Modal isOpen={this.props.ShowWaitDialog}>
                        <ModalHeader tag="h2">Please wait...</ModalHeader>
                        <ModalBody>
                            <span className="font-small">Your request may take a while.</span>
                        </ModalBody>
                    </Modal>

                    <Modal isOpen={!this.props.IsConnected}>
                        <ModalHeader tag="h2">Ooops!</ModalHeader>
                        <ModalBody>
                            <span className="font-small">It seems that the connection to the server cannot be established.</span>
                        </ModalBody>
                    </Modal>
                </div>
                <footer>
                    <Container fluid={true}>
                        <Row><hr /></Row>
                        <Row>
                            <ul>
                                <li>Maintenance {VERSION}</li>
                                <li>&copy; 2018 PHOENIX CONTACT Corporate Technolgy &amp; Value Chain</li>
                            </ul>
                        </Row>
                    </Container>
                </footer>
            </div>
        );
    }

    private loadAndModulesUpdater(): void {
        this.props.RestClient.systemLoad().then((data) => this.props.onUpdateSystemLoad(data));
        this.props.Modules.forEach((module) => {
            this.props.ModulesRestClient.healthState(module.Name).then((data) => this.props.onUpdateModuleHealthState(module.Name, data));
            this.props.ModulesRestClient.notifications(module.Name).then((data) => this.props.onUpdateModuleNotifications(module.Name, data));
        });
    }

    private clockUpdater(): void {
        this.props.RestClient.serverTime().then((data) => {
            if (data.ServerTime === "") {
                this.props.onUpdateIsConnected(false);
                return;
            }

            const serverTime = moment(data.ServerTime).format("YYYY-MM-DD HH:mm:ss");
            this.props.onUpdateIsConnected(true);
            this.props.onUpdateServerTime(serverTime);
        });
    }

    private isRouteActive(route: string): boolean {
        return this.props.location.pathname.startsWith(route);
    }
}

export default withRouter<RouteComponentProps<{}>>(connect<AppPropModel, AppDispatchPropModel>(mapStateToProps, mapDispatchToProps)(App));
