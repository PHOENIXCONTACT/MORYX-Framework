/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiCogs, mdiConsoleLine, mdiHexagon, mdiHexagonMultiple } from "@mdi/js";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import GridLegacy from "@mui/material/GridLegacy";
import Tab from "@mui/material/Tab";
import Tabs from "@mui/material/Tabs";
import * as React from "react";
import { connect } from "react-redux";
import { Link, Route, Routes } from "react-router-dom";
import RoutingMenu from "../../common/components/Menu/RoutingMenu";
import { SectionInfo } from "../../common/components/SectionInfo";
import MenuItemModel from "../../common/models/MenuItemModel";
import MenuModel from "../../common/models/MenuModel";
import { AppState } from "../../common/redux/AppState";
import { ActionType } from "../../common/redux/Types";
import { HealthStateBadge } from "../../dashboard/components/HealthStateBadge";
import ModulesRestClient from "../api/ModulesRestClient";
import IConfig from "../models/Config";
import ServerModuleModel from "../models/ServerModuleModel";
import { updateModules } from "../redux/ModulesActions";
import Module from "./Module";
import ModuleConfiguration from "./ModuleConfiguration";
import ModuleConsole from "./ModuleConsole";

interface ModulesPropModel {
    RestClient: ModulesRestClient;
    Modules: ServerModuleModel[];
    Configs: IConfig[];
}

interface ModulesDispatchPropModel {
    onUpdateModules?(modules: ServerModuleModel[]): void;
}

const mapDispatchToProps = (dispatch: React.Dispatch<ActionType<{}>>): ModulesDispatchPropModel => {
    return {
        onUpdateModules: (modules: ServerModuleModel[]) => dispatch(updateModules(modules)),
    };
};

const mapStateToProps = (state: AppState): ModulesPropModel => {
    return {
        RestClient: state.Modules.RestClient,
        Modules: state.Modules.Modules,
        Configs: state.Modules.Configs
    };
};

interface ModulesStateModel {
    MenuModel: MenuModel;
}

class Modules extends React.Component<ModulesPropModel & ModulesDispatchPropModel, ModulesStateModel> {
    private updateModulesTimer: NodeJS.Timeout;

    constructor(props: ModulesPropModel & ModulesDispatchPropModel) {
        super(props);
        this.state = { MenuModel: { MenuItems: props.Modules.map((module, idx) => Modules.createMenuItem(module)) } };

        this.modulesUpdater = this.modulesUpdater.bind(this);
    }

    public componentDidMount(): void {
        this.updateModulesTimer = setInterval(this.modulesUpdater, 5000);
    }

    public componentWillReceiveProps(nextProps: ModulesPropModel): void {
        this.setState({ MenuModel: { ...this.state.MenuModel, MenuItems: nextProps.Modules.map((module, idx) => Modules.createMenuItem(module)) } });
    }

    public modulesUpdater(): void {
        this.props.RestClient.modules().then((data) => this.props.onUpdateModules(data));
    }

    private static createMenuItem(moduleModel: ServerModuleModel): MenuItemModel {
        return {
            Name: moduleModel.name,
            NavPath: "/modules/" + moduleModel.name,
            Icon: mdiHexagon,
            Content: (<HealthStateBadge HealthState={moduleModel.healthState} />),
            SubMenuItems:
                [
                    {
                        Name: "Configuration",
                        NavPath: "/modules/" + moduleModel.name + "/configuration",
                        Icon: mdiCogs,
                        SubMenuItems: [],
                    },
                    {
                        Name: "Console",
                        NavPath: "/modules/" + moduleModel.name + "/console",
                        Icon: mdiConsoleLine,
                        SubMenuItems: [],
                    },
                ],
        };
    }

    public preRenderRoutesList(): any[] {
        const routes: any[] = [];
        let idx = 0;

        this.state.MenuModel.MenuItems.forEach((menuItem) => {
            const module = this.props.Modules.filter(function (element: ServerModuleModel, index: number, array: ServerModuleModel[]): boolean { return element.name === menuItem.Name; })[0];
            routes.push(
                <Route key={idx} path={menuItem.NavPath.replace("/modules/", "")} element={
                    <Module Module={module} RestClient={this.props.RestClient} />} />);

            menuItem.SubMenuItems.forEach((subMenuItem) => {
                if (subMenuItem.NavPath.endsWith("configuration")) {
                    routes.push(
                        <Route key={idx} path={subMenuItem.NavPath.replace("/modules/", "")} element={
                            <ModuleConfiguration ModuleName={module.name} RestClient={this.props.RestClient} />} />);
                } else if (subMenuItem.NavPath.endsWith("console")) {
                    routes.push(
                        <Route key={idx} path={subMenuItem.NavPath.replace("/modules/", "")} element={
                            <ModuleConsole ModuleName={module.name} RestClient={this.props.RestClient} />} />);
                }

                ++idx;
            });

            ++idx;
        });

        return routes;
    }

    public render(): React.ReactNode {
        return (
            <GridLegacy container={true} spacing={2}>
                <GridLegacy item={true} md={3}>
                    <Card className="mcc-menu-card">
                        <Tabs value="modules" role="navigation" centered={true}>
                            <Tab label="Modules" value="modules" component={Link} to="/modules" />
                            <Tab label="Databases" value="databases" component={Link} to="/databases" />
                        </Tabs>
                        <RoutingMenu Menu={this.state.MenuModel} />
                    </Card>
                </GridLegacy>
                <GridLegacy item={true} md={9}>
                    <Routes>
                        <Route path="*" element={
                            <Card>
                                <CardContent>
                                    <SectionInfo
                                        description="Watch, configure and maintain all available modules. Please select a module to proceed."
                                        icon={mdiHexagonMultiple}
                                    />
                                </CardContent>
                            </Card>} />
                        {this.preRenderRoutesList()}
                    </Routes>
                </GridLegacy>
            </GridLegacy>
        );
    }
}

export default connect<ModulesPropModel, ModulesDispatchPropModel>(mapStateToProps, mapDispatchToProps)(Modules);
