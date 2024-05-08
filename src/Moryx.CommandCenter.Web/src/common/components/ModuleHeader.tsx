/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Tab from "@mui/material/Tab";
import Tabs from "@mui/material/Tabs";
import * as React from "react";
import { connect } from "react-redux";
import { Link } from "react-router-dom";
import { FailureBehaviour } from "../../modules/models/FailureBehaviour";
import { ModuleStartBehaviour } from "../../modules/models/ModuleStartBehaviour";
import { updateFailureBehaviour, updateStartBehaviour } from "../../modules/redux/ModulesActions";
import { ActionType } from "../redux/Types";

const mapDispatchToProps = (dispatch: React.Dispatch<ActionType<{}>>): ModuleDispatchPropModel => {
    return {
        onUpdateStartBehaviour: (moduleName: string, startBehaviour: ModuleStartBehaviour) => dispatch(updateStartBehaviour(moduleName, startBehaviour)),
        onUpdateFailureBehaviour: (moduleName: string, failureBehaviour: FailureBehaviour) => dispatch(updateFailureBehaviour(moduleName, failureBehaviour)),
    };
};

interface ModuleHeaderPropModel {
    ModuleName: string;
    selectedTab: string;
}

export class ModuleHeader extends React.Component<ModuleHeaderPropModel & ModuleDispatchPropModel> {
    constructor(props: ModuleHeaderPropModel & ModuleDispatchPropModel) {
        super(props);
    }

    public render(): React.ReactNode {
        return (
            <Tabs value={this.props.selectedTab} role="navigation">
                <Tab label="Overview" value="module" iconPosition="start" component={Link} to={`/modules/${this.props.ModuleName}`} />
                <Tab label="Configuration" value="configuration" iconPosition="start" component={Link} to={`/modules/${this.props.ModuleName}/configuration`} />
                <Tab label="Console" value="console" iconPosition="start" component={Link} to={`/modules/${this.props.ModuleName}/console`} />
            </Tabs>
        );
    }
}

interface ModuleDispatchPropModel {
    onUpdateStartBehaviour?(moduleName: string, startBehaviour: ModuleStartBehaviour): void;
    onUpdateFailureBehaviour?(moduleName: string, failureBehaviour: FailureBehaviour): void;
}

export default connect<{}, ModuleDispatchPropModel>(null, mapDispatchToProps)(ModuleHeader);
