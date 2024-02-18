/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiBriefcase, mdiComment, mdiDatabase, mdiHexagonMultiple } from "@mdi/js";
import Icon from "@mdi/react";
import * as React from "react";
import { connect } from "react-redux";
import { Link, Route, RouteComponentProps, Switch } from "react-router-dom";
import { Card, CardBody, CardHeader, Col, ListGroup, Nav, Navbar, NavItem, Row } from "reactstrap";
import RoutingMenu from "../../common/components/Menu/RoutingMenu";
import MenuItemModel from "../../common/models/MenuItemModel";
import MenuModel from "../../common/models/MenuModel";
import { AppState } from "../../common/redux/AppState";
import { ActionType } from "../../common/redux/Types";
import DatabasesRestClient from "../api/DatabasesRestClient";
import DataModel from "../models/DataModel";
import { updateDatabaseConfigs } from "../redux/DatabaseActions";
import DatabaseModel from "./DatabaseModel";

interface DatabasesPropsModel {
    RestClient?: DatabasesRestClient;
    DatabaseConfigs?: DataModel[];
}

interface DatabasesDispatchPropModel {
    onUpdateDatabaseConfigs?(databaseConfigs: DataModel[]): void;
}

const mapStateToProps = (state: AppState): DatabasesPropsModel => {
    return {
        RestClient: state.Databases.RestClient,
        DatabaseConfigs: state.Databases.DatabaseConfigs
    };
};

const mapDispatchToProps = (dispatch: React.Dispatch<ActionType<{}>>): DatabasesDispatchPropModel => {
    return {
        onUpdateDatabaseConfigs: (databaseConfigs: DataModel[]) => dispatch(updateDatabaseConfigs(databaseConfigs)),
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
            const validModels = data.databases.filter((model) => model);
            this.props.onUpdateDatabaseConfigs(validModels);
            this.setState({ MenuModel: { MenuItems: validModels.map((dataModel, idx) => Database.createMenuItem(dataModel)) }, IsLoading: false });
        });
    }

    private static createMenuItem(dataModel: DataModel): MenuItemModel {
        const context = dataModel.targetModel.replace(/^.+\./, "");
        const namespace = dataModel.targetModel.replace("." + context, "");
        return {
            Name: context,
            NavPath: "/databases/" + dataModel.targetModel,
            Icon: mdiBriefcase,
            Content: (<p style={{ margin: "inherit", color: "gray", fontSize: "x-small" }}>{namespace}</p>),
            SubMenuItems: [],
        };
    }

    public preRenderRoutesList(): any[] {
        const routes: any[] = [];
        let idx = 0;

        this.props.DatabaseConfigs.forEach((model) => {
            routes.push(
                <Route key={idx} path={`/databases/${model.targetModel}`} exact={true}>
                    <DatabaseModel DataModel={model} RestClient={this.props.RestClient} />
                </Route>);
            ++idx;
        });

        return routes;
    }

    public render(): React.ReactNode {
        return (
            <Row>
                <Col md={3}>
                    <Card>
                        <CardHeader tag="h5">
                            <Navbar className="navbar-default" expand="md">
                                <Nav className="navbar-left" navbar={true}>
                                    <NavItem>
                                        <Link to="/modules" className="navbar-nav-link">
                                            <Icon path={mdiHexagonMultiple} className="icon right-space" />
                                            Modules
                                        </Link>
                                    </NavItem>
                                    <NavItem className="active">
                                        <Link to="/databases" className="navbar-nav-link">
                                            <Icon path={mdiDatabase} className="icon right-space" />
                                            Databases
                                        </Link>
                                    </NavItem>
                                </Nav>
                            </Navbar>
                        </CardHeader>
                        <ListGroup>
                            {this.state.IsLoading ? (
                                <span>Loading...</span>
                            ) : (
                                <RoutingMenu Menu={this.state.MenuModel} />
                            )}
                        </ListGroup>
                    </Card>
                </Col>
                <Col md={9}>
                    <Switch>
                        <Route exact={true} path="/databases">
                            <Card>
                                <CardHeader tag="h4">
                                    <Icon path={mdiComment} className="icon right-space" />
                                    Information
                                </CardHeader>
                                <CardBody>
                                    <span className="font-italic font-small">Configure all available database models. Please select a database model to proceed...</span>
                                </CardBody>
                            </Card>
                        </Route>
                        {this.preRenderRoutesList()}
                    </Switch>
                </Col>
            </Row>
        );
    }
}

export default connect<DatabasesPropsModel, DatabasesDispatchPropModel>(mapStateToProps, mapDispatchToProps)(Database);
