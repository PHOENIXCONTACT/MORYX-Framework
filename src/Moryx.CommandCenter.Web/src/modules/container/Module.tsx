/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiCheck, mdiPlay, mdiRestart, mdiStop } from "@mdi/js";
import Button from "@mui/material/Button";
import ButtonGroup from "@mui/material/ButtonGroup";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import GridLegacy from "@mui/material/GridLegacy";
import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemText from "@mui/material/ListItemText";
import MenuItem from "@mui/material/MenuItem";
import SvgIcon from "@mui/material/SvgIcon";
import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";
import * as React from "react";
import { connect } from "react-redux";
import { NavLink } from "react-router-dom";
import { toast } from "react-toastify";
import ModuleHeader from "../../common/components/ModuleHeader";
import { ActionType } from "../../common/redux/Types";
import { HealthStateBadge } from "../../dashboard/components/HealthStateBadge";
import ModulesRestClient from "../api/ModulesRestClient";
import { FailureBehaviour } from "../models/FailureBehaviour";
import { ModuleStartBehaviour } from "../models/ModuleStartBehaviour";
import NotificationModel from "../models/NotificationModel";
import ServerModuleModel from "../models/ServerModuleModel";
import { Serverity } from "../models/Severity";
import { updateFailureBehaviour, updateStartBehaviour } from "../redux/ModulesActions";
import { ModuleInfoTile } from "./ModuleInfoTile";
import { Notifications } from "./Notifications";

interface ModulePropModel {
    RestClient?: ModulesRestClient;
    Module: ServerModuleModel;
}

interface ModuleStateModel {
    HasWarningsOrErrors: boolean;
    IsNotificationDialogOpened: boolean;
    SelectedNotification: NotificationModel;
}

interface ModuleDispatchPropModel {
    onUpdateStartBehaviour?(moduleName: string, startBehaviour: ModuleStartBehaviour): void;
    onUpdateFailureBehaviour?(moduleName: string, failureBehaviour: FailureBehaviour): void;
}

const mapDispatchToProps = (dispatch: React.Dispatch<ActionType<{}>>): ModuleDispatchPropModel => {
    return {
        onUpdateStartBehaviour: (moduleName: string, startBehaviour: ModuleStartBehaviour) => dispatch(updateStartBehaviour(moduleName, startBehaviour)),
        onUpdateFailureBehaviour: (moduleName: string, failureBehaviour: FailureBehaviour) => dispatch(updateFailureBehaviour(moduleName, failureBehaviour)),
    };
};

class Module extends React.Component<ModulePropModel & ModuleDispatchPropModel, ModuleStateModel> {

    constructor(props: ModulePropModel & ModuleDispatchPropModel) {
        super(props);

        this.state = { HasWarningsOrErrors: false, IsNotificationDialogOpened: false, SelectedNotification: null };
    }

    public componentWillReceiveProps(nextProps: ModulePropModel): void {
        const warningsOrErrors = nextProps.Module.notifications.filter(function (element: NotificationModel, index: number, array: NotificationModel[]): boolean {
            return element.severity === Serverity.Warning || element.severity === Serverity.Error || element.severity === Serverity.Fatal;
        });
        this.setState({ HasWarningsOrErrors: warningsOrErrors.length !== 0 });
    }

    public startModule(): void {
        this.props.RestClient.startModule(this.props.Module.name);
    }

    public stopModule(): void {
        this.props.RestClient.stopModule(this.props.Module.name);
    }

    public reincarnateModule(): void {
        this.props.RestClient.reincarnateModule(this.props.Module.name).then((result) => toast.info("Module is restarting...", { autoClose: 5000 }));
    }

    public confirmModuleWarning(): void {
        this.props.RestClient.confirmModuleWarning(this.props.Module.name);
    }

    public onStartBehaviourChange(e: React.ChangeEvent<HTMLInputElement>): void {
        const newValue = e.target.value as ModuleStartBehaviour;
        this.props.RestClient.updateModule({ ...this.props.Module, startBehaviour: newValue }).then((d) => this.props.onUpdateStartBehaviour(this.props.Module.name, newValue));
    }

    public onFailureBehaviourChange(e: React.ChangeEvent<HTMLInputElement>): void {
        const newValue = e.target.value as FailureBehaviour;
        this.props.RestClient.updateModule({ ...this.props.Module, failureBehaviour: newValue }).then((d) => this.props.onUpdateFailureBehaviour(this.props.Module.name, newValue));
    }

    private static dependenciesList(module: ServerModuleModel): React.ReactNode {
        return (<List dense={true} disablePadding={true}>
            {module.dependencies.map((module, idx) => {
                return [
                    <ListItem
                        secondaryAction={<HealthStateBadge HealthState={module.healthState} />}
                        disablePadding={true}
                        component={NavLink} to={"/modules/" + module.name} sx={{color: "black"}}
                        key={idx}
                    >
                        <ListItemButton
                            divider={idx < module.dependencies.length - 1}
                        >
                        <ListItemText secondary={false} primary={module.name} />
                        </ListItemButton>
                    </ListItem>
                ];
            })}
            </List>
        );
    }

    public render(): React.ReactNode {
        const svgIcon = (path: string) => {
            return (
                <SvgIcon>
                    <path d={path} />
                </SvgIcon>
            );
        };

        return (
            <Card>
                <ModuleHeader ModuleName={this.props.Module.name} selectedTab="module" />
                <CardContent>
                    <GridLegacy container={true} spacing={2} direction="row" alignItems="stretch">
                        <ModuleInfoTile
                            title={this.props.Module.name}
                        >
                            <GridLegacy item={true}>
                                <ButtonGroup variant="contained" sx={{flex: "none"}} fullWidth={false}>
                                    <Button onClick={this.startModule.bind(this)} startIcon={svgIcon(mdiPlay)}>Start</Button>
                                    <Button onClick={this.stopModule.bind(this)} startIcon={svgIcon(mdiStop)}>Stop</Button>
                                    <Button onClick={this.reincarnateModule.bind(this)} startIcon={svgIcon(mdiRestart)}>Reincarnate</Button>
                                </ButtonGroup>
                            </GridLegacy>
                        </ModuleInfoTile>
                        <ModuleInfoTile
                            title="Error Handling"
                        >
                            <GridLegacy item={true} justifyContent="center">
                                {this.state.HasWarningsOrErrors ? (
                                    <Button
                                        variant="outlined"
                                        color="warning"
                                        onClick={this.confirmModuleWarning.bind(this)}
                                        startIcon={<SvgIcon><path d={mdiCheck} /></SvgIcon>}
                                    >
                                        Confirm
                                    </Button>
                                ) : (
                                    <Typography variant="body2" gutterBottom={true}>No warnings or errors.</Typography>
                                )}
                            </GridLegacy>
                        </ModuleInfoTile>
                        <ModuleInfoTile
                            title="General Information"
                            spacing={1}
                        >
                            <GridLegacy item={true}>
                                <List dense={true} disablePadding={true}>
                                    <ListItem  disablePadding={true}>
                                        <ListItemText primary={<HealthStateBadge HealthState={this.props.Module.healthState} />} />
                                    </ListItem>
                                    <ListItem  disablePadding={true}>
                                        <ListItemText primary={this.props.Module.assembly.name} secondary="Assembly" />
                                    </ListItem>
                                    <ListItem  disablePadding={true}>
                                        <ListItemText primary={this.props.Module.assembly.fileVersion} secondary="Version" />
                                    </ListItem>
                                </List>
                            </GridLegacy>
                        </ModuleInfoTile>

                        <ModuleInfoTile
                            title="Dependencies"
                        >
                            <GridLegacy>
                                {
                                this.props.Module.dependencies.length === 0 ? (
                                    <span>This module has no dependencies.</span>
                                ) : (
                                    Module.dependenciesList(this.props.Module)
                                )
                                }
                            </GridLegacy>
                        </ModuleInfoTile>
                        <ModuleInfoTile
                            title="Start &amp; Failure behaviour"
                        >
                            <GridLegacy item={true} md={12}>
                                <TextField
                                    select={true}
                                    label="Start behaviour"
                                    value={this.props.Module.startBehaviour}
                                    onChange={(e: React.ChangeEvent<HTMLInputElement>) => this.onStartBehaviourChange(e)}
                                    size="small"
                                    margin="dense"
                                    fullWidth={true}>
                                    <MenuItem value={ModuleStartBehaviour.Auto}>Auto</MenuItem>
                                    <MenuItem value={ModuleStartBehaviour.Manual}>Manual</MenuItem>
                                    <MenuItem value={ModuleStartBehaviour.OnDependency}>On dependency</MenuItem>
                                </TextField>
                            </GridLegacy>

                            <GridLegacy item={true} md={12}>
                                <TextField
                                    select={true}
                                    label="Failure behaviour"
                                    value={this.props.Module.failureBehaviour}
                                    onChange={(e: React.ChangeEvent<HTMLInputElement>) => this.onFailureBehaviourChange(e)}
                                    size="small"
                                    margin="dense"
                                    fullWidth={true}
                                    >
                                    <MenuItem value={FailureBehaviour.Stop}>Stop</MenuItem>
                                    <MenuItem value={FailureBehaviour.StopAndNotify}>Stop and notify</MenuItem>
                                </TextField>
                            </GridLegacy>
                        </ModuleInfoTile>
                        <ModuleInfoTile
                                title="Notifications"
                                md={12}
                            >
                                <Notifications messages={this.props.Module.notifications}/>

                        </ModuleInfoTile>
                    </GridLegacy>
                </CardContent>
            </Card>
        );
    }
}

export default connect<{}, ModuleDispatchPropModel>(null, mapDispatchToProps)(Module);
