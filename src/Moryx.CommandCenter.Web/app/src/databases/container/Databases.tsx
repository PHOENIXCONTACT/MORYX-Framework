/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiBriefcase, mdiComment, mdiDatabase } from "@mdi/js";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import GridLegacy from "@mui/material/GridLegacy";
import List from "@mui/material/List";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemText from "@mui/material/ListItemText";
import Skeleton from "@mui/material/Skeleton";
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
import DatabasesRestClient from "../api/DatabasesRestClient";
import DatabaseAndConfigurators from "../models/DatabaseAndConfigurators";
import DatabasesResponse from "../models/DatabasesResponse";
import DataModel from "../models/DataModel";
import {ModelConfiguratorModel} from "../models/ModelConfiguratorModel";
import { updateDatabases } from "../redux/DatabaseActions";
import DatabaseModel from "./DatabaseModel";

interface DatabasesPropsModel {
    RestClient?: DatabasesRestClient;
    Databases?: DatabaseAndConfigurators[];
}

interface DatabasesDispatchPropModel {
    onUpdateDatabases?(databasesAndConfigurators: DatabaseAndConfigurators[]): void;
}

const mapStateToProps = (state: AppState): DatabasesPropsModel => {
    return {
        RestClient: state.Databases.RestClient,
        Databases: state.Databases.Databases
    };
};

const mapDispatchToProps = (dispatch: React.Dispatch<ActionType<{}>>): DatabasesDispatchPropModel => {
    return {
        onUpdateDatabases: (databasesAndConfigurators: DatabaseAndConfigurators[]) => dispatch(updateDatabases(databasesAndConfigurators)),
    };
};

interface DatabaseStateModel {
    MenuModel: MenuModel;
    IsLoading: boolean;
}

class Database extends React.Component<DatabasesPropsModel & DatabasesDispatchPropModel, DatabaseStateModel> {
    constructor(props: DatabasesPropsModel & DatabasesDispatchPropModel) {
        super(props);
        this.state = { MenuModel: { MenuItems: [] }, IsLoading: false };
    }

    public componentDidMount(): void {
        this.loadDatabases();
    }

    private loadDatabases(): void {
        this.setState({ IsLoading: true });

        this.props.RestClient.databaseModels().then((data) => {
            console.log(data);
            const validModels = data.databases.filter((model) => model);
            this.props.onUpdateDatabases(this.mapDatabasesWithConfigurators(data));
            this.setState({ MenuModel: { MenuItems: validModels.map((dataModel, idx) => Database.createMenuItem(dataModel)) }, IsLoading: false });
        });
    }

    private mapDatabasesWithConfigurators(response: DatabasesResponse): DatabaseAndConfigurators[] {
        const configuratorByType = new Map<string, ModelConfiguratorModel[]>();

        for (const configurator of response.configurators) {
            const list = configuratorByType.get(configurator.configuratorType) ?? [];
            list.push(configurator);
            configuratorByType.set(configurator.configuratorType, list);
        }

        return response.databases.map((database) => ({
            database,
            configurators: database.possibleConfigurators
                .flatMap((type) => configuratorByType.get(type) ?? [])
        }));
    }

    private static createMenuItem(dataModel: DataModel): MenuItemModel {
        const context = dataModel.targetModel.replace(/^.+\./, "");
        const namespace = dataModel.targetModel.replace("." + context, "");
        return {
            Name: context,
            NavPath: "/databases/" + dataModel.targetModel,
            Icon: mdiBriefcase,
            SecondaryName: namespace,
            SubMenuItems: [],
        };
    }

    public preRenderRoutesList(): any[] {
        const routes: any[] = [];
        let idx = 0;

        this.props.Databases.forEach((databaseAndConfigurator) => {
            routes.push(
                <Route key={idx} path={`${databaseAndConfigurator.database.targetModel}`} element={
                    <DatabaseModel DataModel={databaseAndConfigurator.database} RestClient={this.props.RestClient} ModelConfigurators={databaseAndConfigurator.configurators} />} />);
            ++idx;
        });

        return routes;
    }

    public render(): React.ReactNode {
        return (
            <GridLegacy container={true} spacing={2}>
                <GridLegacy item={true} md={3}
                    justifyContent={"center"}>
                    <Card className="mcc-menu-card">
                            <Tabs value="databases" role="navigation" centered={true}>
                                <Tab label="Modules" value="modules" component={Link} to="/modules" />
                                <Tab label="Databases" value="databases" component={Link} to="/databases" />
                            </Tabs>
                        {this.state.IsLoading ? (
                            <List>
                                <ListItemButton
                                    className="menu-item"
                                    divider={true}
                                    disabled={true}>
                                    <ListItemText>
                                        <Skeleton animation="wave" variant="text" sx={{width: "70%", fontSize: "1.2rem"}} />
                                        <Skeleton animation="wave" variant="text" sx={{width: "95%", fontSize: "0.5rem"}} />
                                    </ListItemText>
                                </ListItemButton>
                                <ListItemButton
                                    className="menu-item"
                                    divider={true}
                                    disabled={true}>
                                    <ListItemText>
                                        <Skeleton animation="wave" variant="text" sx={{width: "70%", fontSize: "1.2rem"}} />
                                        <Skeleton animation="wave" variant="text" sx={{width: "95%", fontSize: "0.5rem"}} />
                                    </ListItemText>
                                </ListItemButton>
                            </List>
                        ) : (
                            <RoutingMenu Menu={this.state.MenuModel} />
                        )}
                    </Card>
                </GridLegacy>
                <GridLegacy item={true} md={9}>
                    <Routes>
                        <Route path="*" element={
                            <Card>
                                <CardContent>
                                    <SectionInfo
                                        description="Configure all available database models. Please select a database model to proceed."
                                        icon={mdiDatabase}
                                    />
                                </CardContent>
                            </Card>}
                        />
                        {this.preRenderRoutesList()}
                    </Routes>
                </GridLegacy>
            </GridLegacy>
        );
    }
}

export default connect<DatabasesPropsModel, DatabasesDispatchPropModel>(mapStateToProps, mapDispatchToProps)(Database);
