/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiFormatListBulletedSquare, mdiSquareEditOutline } from "@mdi/js";
import Icon from "@mdi/react";
import { Location, UnregisterCallback } from "history";
import * as React from "react";
import NotificationSystem = require("react-notification-system");
import { connect } from "react-redux";
import { Link, Route, RouteComponentProps, Switch, withRouter } from "react-router-dom";
import { Card, CardBody, CardHeader, Col, Container, Input, Nav, NavItem, NavLink, Row, TabContent, TabPane } from "reactstrap";
import TreeMenu from "../../common/components/Menu/TreeMenu";
import { IconType } from "../../common/models/MenuItemModel";
import MenuModel from "../../common/models/MenuModel";
import { AppState } from "../../common/redux/AppState";
import { ActionType } from "../../common/redux/Types";
import LogRestClient from "../api/LogRestClient";
import Logger from "../components/Logger";
import LogMenuItemContent from "../components/LogMenuItemContent";
import LoggerModel from "../models/LoggerModel";
import { LogLevel } from "../models/LogLevel";
import LogMenuItem from "../models/LogMenuItem";
import { updateLoggers } from "../redux/LogActions";

interface LogPropsModel {
    RestClient?: LogRestClient;
    Loggers?: LoggerModel[];
    NotificationSystem?: NotificationSystem.System;
}

interface LogDispatchPropModel {
    onUpdateLoggers?(loggers: LoggerModel[]): void;
}

const mapStateToProps = (state: AppState): LogPropsModel => {
    return {
        RestClient: state.Log.RestClient,
        Loggers: state.Log.Loggers,
        NotificationSystem: state.Common.NotificationSystem,
    };
};

const mapDispatchToProps = (dispatch: React.Dispatch<ActionType<{}>>): LogDispatchPropModel => {
    return {
        onUpdateLoggers: (loggers: LoggerModel[]) => dispatch(updateLoggers(loggers)),
    };
};

interface LogStateModel {
    ActiveTab: string;
    Menu: MenuModel;
    LoggerTabs: LoggerModel[];
}

class Log extends React.Component<LogPropsModel & LogDispatchPropModel, LogStateModel> {

    private overviewLogger: LoggerModel;

    constructor(props: LogPropsModel & LogDispatchPropModel) {
        super(props);
        this.state = { ActiveTab: "0", Menu: { MenuItems: [] }, LoggerTabs: [] };

        this.overviewLogger = new LoggerModel();
        this.overviewLogger.Name = "";
    }

    public componentDidMount(): void {
        this.props.RestClient.loggers().then((data) => {
            this.props.onUpdateLoggers(data);
            this.setState({Menu: { MenuItems: data.map((logger, idx) => this.createMenuItem(logger)) } });
        });
    }

    public createMenuItem(logger: LoggerModel): LogMenuItem {
        const menuItem: LogMenuItem = {
                Name: LoggerModel.shortLoggerName(logger),
                NavPath: "/log",
                Logger: logger,
                SubMenuItems: logger.ChildLogger.map((childLogger, idx) => this.createMenuItem(childLogger)),
            };

        menuItem.Content = (<LogMenuItemContent Logger={menuItem.Logger}
                                                onActiveLogLevelChange={this.onActiveLogLevelChange.bind(this)}
                                                onLabelClicked={this.onMenuItemClicked.bind(this)} />);
        return menuItem;
    }

    private onActiveLogLevelChange(e: React.FormEvent<HTMLInputElement>, logger: LoggerModel): void {
        e.preventDefault();

        const newValue = parseInt((e.target as HTMLSelectElement).value, 10);
        this.props.RestClient.logLevel(logger.Name, newValue).then((data) => {
            if (data.Success) {
                logger.ActiveLevel = newValue;
                Log.changeActiveLogLevel(logger, newValue);
                this.forceUpdate();

                this.props.NotificationSystem.addNotification({ title: "Success", message: "Log level for '" + logger.Name +  "' was set successfully", level: "success", autoDismiss: 5 });
            } else {
                this.props.NotificationSystem.addNotification({ title: "Error", message: data.ErrorMessage, level: "error", autoDismiss: 5 });
            }
        });
    }

    private static changeActiveLogLevel(logger: LoggerModel, logLevel: LogLevel): void {
        if (logger.ActiveLevel > logLevel) {
            logger.ActiveLevel = logLevel;
        }

        for (const childLogger of logger.ChildLogger) {
            Log.changeActiveLogLevel(childLogger, logLevel);
        }
    }

    private toggleTab(tabName: string): void {
        this.setState({ActiveTab: tabName});
    }

    private onMenuItemClicked(logger: LoggerModel): void {
       const idx = this.state.LoggerTabs.indexOf(logger);
       if (idx === -1) {
            this.setState((prevState) => ({
                LoggerTabs: [...prevState.LoggerTabs, logger],
                ActiveTab: (prevState.LoggerTabs.length + 1).toString(),
            }));
       } else {
            this.setState({ ActiveTab: (idx + 1).toString() });
       }
    }

    private onCloseTab(logger: LoggerModel): void {
        const idx = this.state.LoggerTabs.indexOf(logger);
        if (idx !== -1) {
            let activeTab = parseInt(this.state.ActiveTab, 10);
            if (activeTab >= this.state.LoggerTabs.length) {
                activeTab -= 1;
            }

            this.setState((prevState) => ({
                LoggerTabs: prevState.LoggerTabs.filter((_, i) => i !== idx),
                ActiveTab: (activeTab).toString(),
            }));
        }
    }

    private preRenderNavForTabs(): React.ReactNode {
        return this.state.LoggerTabs.map((logger, idx) =>
            <NavItem key={idx} className={"selectable"}>
                <NavLink className={this.state.ActiveTab === (idx + 1).toString() ? "active" : ""} onClick={() => { this.toggleTab((idx + 1).toString()); }}>
                    {LoggerModel.shortLoggerName(logger)}
                </NavLink>
            </NavItem>,
        );
    }

    private preRenderTabs(): React.ReactNode {
        return this.state.LoggerTabs.map((logger, idx) =>
            <TabPane key={idx} tabId={(idx + 1).toString()}>
                <Row>
                    <Col md="12">
                        <Logger RestClient={this.props.RestClient} Logger={logger} onCloseTab={this.onCloseTab.bind(this)} />
                    </Col>
                </Row>
            </TabPane>,
        );
    }

    public render(): React.ReactNode {
        return (
            <Row>
                <Col md={3}>
                    <Card>
                        <CardHeader tag="h2">
                            <Icon path={mdiFormatListBulletedSquare} className="icon right-space" />
                            Loggers
                        </CardHeader>
                        <CardBody>
                            <Container fluid={true}>
                                <TreeMenu Menu={this.state.Menu} />
                            </Container>
                        </CardBody>
                    </Card>
                </Col>
                <Col md={9}>
                    <Card>
                        <CardHeader tag="h2">
                            <Icon path={mdiSquareEditOutline} className="icon right-space" />
                            Log
                        </CardHeader>
                        <CardBody>
                            <Nav tabs={true}>
                                <NavItem>
                                    <NavLink className={this.state.ActiveTab === "0" ? "active selectable" : "selectable"} onClick={() => { this.toggleTab("0"); }}>
                                        Overview
                                    </NavLink>
                                </NavItem>
                                {this.preRenderNavForTabs()}
                            </Nav>
                            <TabContent activeTab={this.state.ActiveTab}>
                                <TabPane tabId="0">
                                    <Row>
                                        <Col md="12">
                                            <Logger RestClient={this.props.RestClient} Logger={this.overviewLogger} onCloseTab={null} />
                                        </Col>
                                    </Row>
                                </TabPane>
                                {this.preRenderTabs()}
                            </TabContent>
                        </CardBody>
                    </Card>
                </Col>
            </Row>
        );
    }
}

export default connect<LogPropsModel, LogDispatchPropModel>(mapStateToProps, mapDispatchToProps)(Log);
