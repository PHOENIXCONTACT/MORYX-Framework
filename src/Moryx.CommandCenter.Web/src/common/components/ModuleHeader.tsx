/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiCogs, mdiConsole, mdiConsoleLine, mdiDatabase, mdiHexagon, mdiHexagonMultiple, mdiMonitor } from "@mdi/js";
import Icon from "@mdi/react";
import * as React from "react";
import { connect } from "react-redux";
import { Link, NavLink } from "react-router-dom";
import { Nav, Navbar, NavItem } from "reactstrap";
import { FailureBehaviour } from "../../modules/models/FailureBehaviour";
import { ModuleStartBehaviour } from "../../modules/models/ModuleStartBehaviour";
import ServerModuleModel from "../../modules/models/ServerModuleModel";
import { updateFailureBehaviour, updateStartBehaviour } from "../../modules/redux/ModulesActions";
import { AppState } from "../redux/AppState";
import { ActionType } from "../redux/Types";

interface ModuleHeaderPropModel {
    Module?: ServerModuleModel;
}

const mapStateToProps = (state: AppState): ModuleHeaderPropModel => {
    return {
    };
};

const mapDispatchToProps = (dispatch: React.Dispatch<ActionType<{}>>): ModuleDispatchPropModel => {
    return {
        onUpdateStartBehaviour: (moduleName: string, startBehaviour: ModuleStartBehaviour) => dispatch(updateStartBehaviour(moduleName, startBehaviour)),
        onUpdateFailureBehaviour: (moduleName: string, failureBehaviour: FailureBehaviour) => dispatch(updateFailureBehaviour(moduleName, failureBehaviour)),
    };
};

interface ModulePropModel {
    ModuleName: string;
}

export class ModuleHeader extends React.Component<ModulePropModel & ModuleDispatchPropModel> {
    constructor(props: ModulePropModel & ModuleDispatchPropModel) {
        super(props);

        // This.state = { HasWarningsOrErrors: false, IsNotificationDialogOpened: false, SelectedNotification: null };
    }

    public render(): React.ReactNode {
        return (
            <Navbar className="navbar-default" expand="md">
                <Nav className="navbar-left" navbar={true}>
                    <NavItem>
                        <NavLink end={true} to={`/modules/${this.props.ModuleName}`} className="navbar-nav-link">
                            <Icon path={mdiMonitor} className="icon right-space" />
                            Overview
                        </NavLink>
                    </NavItem>
                    <NavItem>
                        <NavLink to={`/modules/${this.props.ModuleName}/configuration`} className="navbar-nav-link">
                            <Icon path={mdiCogs} className="icon right-space" />
                            Configuration
                        </NavLink>
                    </NavItem>
                    <NavItem >
                        <NavLink to={`/modules/${this.props.ModuleName}/console`} className="navbar-nav-link">
                            <Icon path={mdiConsoleLine} className="icon right-space" />
                            Console
                        </NavLink>
                    </NavItem>
                </Nav>
            </Navbar>
        );
    }
}

interface ModuleDispatchPropModel {
    onUpdateStartBehaviour?(moduleName: string, startBehaviour: ModuleStartBehaviour): void;
    onUpdateFailureBehaviour?(moduleName: string, failureBehaviour: FailureBehaviour): void;
}

export default connect<{}, ModuleDispatchPropModel>(null, mapDispatchToProps)(ModuleHeader);
