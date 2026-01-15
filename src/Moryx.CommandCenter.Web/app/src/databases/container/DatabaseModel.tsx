/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiArrowUpBoldCircleOutline, mdiCogPlayOutline, mdiConnection, mdiDatabaseAlert, mdiDatabaseCheckOutline, mdiTableAlert } from "@mdi/js";
import Button from "@mui/material/Button";
import ButtonGroup from "@mui/material/ButtonGroup";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import CircularProgress from "@mui/material/CircularProgress";
import GridLegacy from "@mui/material/GridLegacy";
import MenuItem from "@mui/material/MenuItem";
import Stack from "@mui/material/Stack";
import SvgIcon from "@mui/material/SvgIcon";
import Tab from "@mui/material/Tab";
import Tabs from "@mui/material/Tabs";
import TextField from "@mui/material/TextField";
import Tooltip from "@mui/material/Tooltip";
import Typography from "@mui/material/Typography";
import * as React from "react";
import { connect } from "react-redux";
import { toast } from "react-toastify";
import { updateShowWaitDialog } from "../../common/redux/CommonActions";
import { ActionType } from "../../common/redux/Types";
import { getEnumTypeValue } from "../../modules/converter/EnumTypeHelper";
import Entry from "../../modules/models/Entry";
import DatabasesRestClient from "../api/DatabasesRestClient";
import { MigrationResult } from "../api/responses/MigrationResult";
import DatabaseConfigModel from "../models/DatabaseConfigModel";
import DataModel from "../models/DataModel";
import DbMigrationsModel from "../models/DbMigrationsModel";
import {ModelConfiguratorModel} from "../models/ModelConfiguratorModel";
import SetupModel from "../models/SetupModel";
import { TestConnectionResult } from "../models/TestConnectionResult";
import { updateDatabaseConfig } from "../redux/DatabaseActions";
import { DatabaseSection } from "./DatabaseSection";
import { ExecuterList } from "./ExecuterList";

interface DatabaseModelPropsModel {
    RestClient: DatabasesRestClient;
    DataModel: DataModel;
    ModelConfigurators: ModelConfiguratorModel[];
}

interface DatabaseModelStateModel {
    activeTab: number;
    config: DatabaseConfigModel;
    targetModel: string;
    modelConfigurator: ModelConfiguratorModel;
    testConnectionPending: boolean;
    testConnectionResult: TestConnectionResult;
    connectionString: string;
}

interface DatabaseModelDispatchPropsModel {
    onUpdateDatabaseConfig?(databaseConfig: DataModel): void;
    onShowWaitDialog?(showWaitDialog: boolean): void;
}

const mapDispatchToProps = (dispatch: React.Dispatch<ActionType<{}>>): DatabaseModelDispatchPropsModel => {
    return {
        onUpdateDatabaseConfig: (databaseConfig: DataModel) => dispatch(updateDatabaseConfig(databaseConfig)),
        onShowWaitDialog: (showWaitDialog: boolean) => dispatch(updateShowWaitDialog(showWaitDialog)),
    };
};

class DatabaseModel extends React.Component<DatabaseModelPropsModel & DatabaseModelDispatchPropsModel, DatabaseModelStateModel> {

    constructor(props: DatabaseModelPropsModel & DatabaseModelDispatchPropsModel) {
        super(props);

        this.state = {
            activeTab: 1,
            config: props.DataModel.config,
            targetModel: props.DataModel.targetModel,
            modelConfigurator: props.ModelConfigurators.find((c) => c.configuratorType === props.DataModel.config.configuratorType)!,
            testConnectionPending : false,
            testConnectionResult : TestConnectionResult.ConfigurationError,
            connectionString: this.props.DataModel.config.connectionString
        };

        this.activeTab = this.activeTab.bind(this);
    }

    public componentDidMount(): void {
        this.onTestConnection();
    }

    public componentDidUpdate(prevProps: DatabaseModelPropsModel, prevState: DatabaseModelStateModel): void {
        if (prevState.targetModel !== this.props.DataModel.targetModel) {
            this.setState({
                config: this.props.DataModel.config,
                targetModel: this.props.DataModel.targetModel,
                connectionString: this.props.DataModel.config.connectionString,
                modelConfigurator: this.props.ModelConfigurators.find((c) => c.configuratorType === this.props.DataModel.config.configuratorType)!
            });
            this.onTestConnection();
        }
    }

    public activeTab(event: React.SyntheticEvent, tabId: number): void {
        this.setState({ activeTab: tabId });
    }

    public onConfiguratorTypeChanged(e: React.ChangeEvent<HTMLInputElement>): void {
        const modelConfigurator = this.props.ModelConfigurators.find((c) => c.configuratorType === e.target.value);
        if (!modelConfigurator) {
            return;
        }

        const config = { ...this.state.config };
        config.configuratorType = modelConfigurator.configuratorType;
        config.connectionString = modelConfigurator.connectionStringPrototype;
        config.properties = { ...modelConfigurator.configPrototype};

        this.setState({
            modelConfigurator,
            connectionString: modelConfigurator.connectionStringPrototype,
            config
        });
    }

    public onConnectionStringChanged(e: React.ChangeEvent<HTMLInputElement>): void {
        const config = { ...this.state.config };
        const newConnectionString = e.target.value;
        config.connectionString = newConnectionString;

        const parts = newConnectionString.split(";").filter((p) => p.trim() !== "");

        parts.map((part) => {
            const [k, v] = part.split("=");

            // Find corresponding entry
            const entry = Object.values(config.properties.subEntries).find((e) => {
                const key = this.state.modelConfigurator.connectionStringKeys[e.identifier];
                return key && key.trim().toLowerCase() === k.trim().toLowerCase();
            });

            // Update if found
            if (entry) {
                entry.value.current = v;
            }
        });

        this.setState({ config, connectionString: newConnectionString });
    }

    public onInputChanged(e: string, entry: Entry): void {
        entry.value.current = e;

        const currentConnectionString = this.state.connectionString;
        const key = this.state.modelConfigurator.connectionStringKeys[entry.identifier];
        const newValue = entry.value.current;

        if (!key) // Safety check
        {
            return;
        }

        // Split connection string into key-value pairs
        const parts = currentConnectionString.split(";").filter((p) => p.trim() !== "");

        // Map and replace the target key
        const updatedParts = parts.map((part) => {
            const [k, v] = part.split("=");
            if (k.trim().toLowerCase() === key.toLowerCase()) {
                return `${k}=${newValue}`; // Update value
            }
            return part; // Keep unchanged
        });

        // Join back into a single string
        const updatedConnectionString = updatedParts.join(";");

        const config = { ...this.state.config };
        config.connectionString = updatedConnectionString;

        // Update state
        this.setState({connectionString: updatedConnectionString, config});
    }

    private createEntriesInput(): React.JSX.Element[] {
        return this.state.config.properties.subEntries.map((entry) => {
            return (
                <TextField
                    key={entry.identifier + "-input"}
                    label={entry.displayName}
                    placeholder={entry.value.default}
                    value={entry.value.current}
                    onBlur={() => this.onTestConnection()}
                    onChange={(e) => this.onInputChanged(e.target.value, entry)}
                    variant="outlined"
                    size="small"
                    margin="dense"
                    />
            );
        });
    }

    public onSave(): void {
        this.props.onShowWaitDialog(true);

        this.onTestConnection();
        this.props.RestClient.saveDatabaseConfig(this.state.config, this.state.targetModel).then((response) => {
            this.props.onShowWaitDialog(false);

            this.setState({ config: response.config, connectionString: response.config.connectionString });
            this.props.onUpdateDatabaseConfig(response);
            toast.success("Configuration saved", { autoClose: 5000 });
            this.onTestConnection();
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onTestConnection(): void {
        this.setState({ testConnectionPending: true });
        this.props.RestClient.testDatabaseConfig(this.state.config, this.props.DataModel.targetModel)
            .then((response) => {
                this.setState({
                    testConnectionPending: false,
                    testConnectionResult: response.result !== undefined ? getEnumTypeValue(TestConnectionResult, response.result) : TestConnectionResult.ConnectionError
                });
            });
    }

    public onCreateDatabase(): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.createDatabase(this.state.config, this.props.DataModel.targetModel).then((data) => {
            this.props.onShowWaitDialog(false);

            if (data.success) {
                this.props.RestClient.databaseModel(this.state.targetModel).then((databaseConfig) => this.props.onUpdateDatabaseConfig(databaseConfig));
                toast.success("Database created successfully", { autoClose: 5000 });
            } else {
                toast.error("Database not created: " + data.errorMessage, { autoClose: 5000 });
            }
            this.onTestConnection();
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onEraseDatabase(): void {
        if (confirm("Do you really want to delete the entire database?") === false) {
            return;
        }

        this.props.onShowWaitDialog(true);

        this.props.RestClient.eraseDatabase(this.state.config, this.props.DataModel.targetModel).then((data) => {
            this.props.onShowWaitDialog(false);

            if (data.success) {
                this.props.RestClient.databaseModel(this.props.DataModel.targetModel).then((databaseConfig) => this.props.onUpdateDatabaseConfig(databaseConfig));
                toast.success("Database deleted successfully", { autoClose: 5000 });
            } else {
                toast.error("Database not deleted: " + data.errorMessage, { autoClose: 5000 });
            }
            this.onTestConnection();
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onApplyMigration(migration: string): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.applyMigration(this.props.DataModel.targetModel, migration, this.state.config).then((data) => {
            this.props.onShowWaitDialog(false);

            if (data.result === MigrationResult.Migrated) {
                this.props.RestClient.databaseModel(this.props.DataModel.targetModel).then((databaseConfig) => this.props.onUpdateDatabaseConfig(databaseConfig));
                toast.success("Migration applied", { autoClose: 5000 });
            } else {
                const errors = data.errors.join("; ");
                toast.error("Migration not applied: " + errors, { autoClose: 5000 });
            }
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onRollbackDatabase(): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.rollbackDatabase(this.props.DataModel.targetModel, this.state.config).then((data) => {
            this.props.onShowWaitDialog(false);

            if (data.success) {
                this.props.RestClient.databaseModel(this.props.DataModel.targetModel).then((databaseConfig) => this.props.onUpdateDatabaseConfig(databaseConfig));
                toast.success("Database rollback completed successfully", { autoClose: 5000 });
            } else {
                toast.error("Database rollback failed: " + data.errorMessage, { autoClose: 5000 });
            }
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onExecuteSetup(setup: SetupModel): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.executeSetup(this.props.DataModel.targetModel, { Config: this.state.config, Setup: setup }).then((data) => {
            this.props.onShowWaitDialog(false);

            if (data.success) {
                toast.success("Setup '" + setup.name + "' executed successfully", { autoClose: 5000 });
            } else {
                toast.error(data.errorMessage, { autoClose: 5000 });
            }
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    private preRenderConnectionCheckIcon(): React.ReactNode {
        switch (this.state.testConnectionResult) {
            case TestConnectionResult.Success:
                return (
                    <SvgIcon color="success"><path d={mdiDatabaseCheckOutline} /></SvgIcon>
                );
            case TestConnectionResult.ConfigurationError:
                    return (
                    <Tooltip title="Please check if the model configuration exists on the server." placement="right" disableInteractive={true}>
                        <SvgIcon color="error"><path d={mdiTableAlert} /></SvgIcon>
                    </Tooltip>
            );
            case TestConnectionResult.ConnectionError:
                return (
                    <Tooltip title="Please check database name and connection string." placement="right">
                        <SvgIcon color="error"><path d={mdiConnection} /></SvgIcon>
                    </Tooltip>
                );
            case TestConnectionResult.ConnectionOkDbDoesNotExist:
                return (
                    <Tooltip title="The connection to the database could be established but the database could not be found. Please check the name of the database and if it exists." placement="right">
                        <SvgIcon color="error"><path d={mdiDatabaseAlert} /></SvgIcon>
                    </Tooltip>
                );
            default:
                return (<div />);
        }
    }

    private getMigrations(): DbMigrationsModel[] {
        return this.props.DataModel.availableMigrations;
    }

    public render(): React.ReactNode {
        return (
            <Card>
                <CardContent>
                    <GridLegacy container={true} direction="column" spacing={1}>
                        <DatabaseSection title={(
                            <Typography variant="h5" gutterBottom={true}>
                                {contextNameWithoutNamespace(this.state.targetModel)} {this.state.testConnectionPending
                                    ? (<CircularProgress size="small"/>)
                                    : this.preRenderConnectionCheckIcon()}
                            </Typography>
                            )}
                        >
                            <GridLegacy item={true} md={12}>
                                <Stack spacing={1}>
                                    <TextField
                                        label="Configurator"
                                        onChange={(e: React.ChangeEvent<HTMLInputElement>) => this.onConfiguratorTypeChanged(e)}
                                        value={this.state.modelConfigurator.configuratorType}
                                        onBlur={() => this.onTestConnection()}
                                        variant="outlined"
                                        select={true}
                                        size="small">
                                            {this.props.ModelConfigurators.map((configurator, idx) => (
                                                <MenuItem key={idx} value={configurator.configuratorType}>{configurator.name}</MenuItem>))
                                            }
                                    </TextField>
                                    <TextField
                                        key="connectionstr-input"
                                        label="Connection String"
                                        placeholder=""
                                        value={this.state.connectionString}
                                        onBlur={() => this.onTestConnection()}
                                        onChange={(e: React.ChangeEvent<HTMLInputElement>) => this.onConnectionStringChanged(e)}
                                        variant="outlined"
                                        size="small"
                                        margin="dense"
                                    />
                                    {this.createEntriesInput()}
                                </Stack>
                            </GridLegacy>
                            <GridLegacy item={true} md={12}>
                                <Button color="primary" variant="outlined" onClick={() => this.onSave()}>Save</Button>
                            </GridLegacy>
                        </DatabaseSection>
                        <DatabaseSection title="Database">
                            <GridLegacy item={true} md={12}>
                                <ButtonGroup>
                                    <Button color="primary"
                                        onClick={() => this.onCreateDatabase()}
                                        disabled={this.state.testConnectionResult !== TestConnectionResult.ConnectionOkDbDoesNotExist}>
                                        Create database
                                    </Button>
                                    <Button color="primary"
                                        onClick={() => this.onEraseDatabase()}
                                        disabled={this.state.testConnectionResult !== TestConnectionResult.Success && this.state.testConnectionResult !== TestConnectionResult.PendingMigrations}>
                                        Erase database
                                    </Button>
                                </ButtonGroup>
                            </GridLegacy>
                            <GridLegacy container={true} item={true}
                                md={12} spacing={1}
                                direction="row"

                            >
                                <GridLegacy item={true} md={12}>
                                    <Tabs value={this.state.activeTab} onChange={this.activeTab}>
                                        <Tab label="Migrations" value={1} />
                                        <Tab label="Setups" value={2} />
                                    </Tabs>
                                </GridLegacy>

                                <GridLegacy container={true} item={true} md={12}
                                    direction="column"
                                    justifyContent="flex-start"
                                    alignItems="stretch"
                                >
                                    <GridLegacy item={true}>
                                        <div hidden={this.state.activeTab !== 1}>
                                            {this.props.DataModel.availableMigrations.length !== 0
                                                ? (<ExecuterList items={
                                                    this.getMigrations().map((migration) => {
                                                        const installed = this.props.DataModel.appliedMigrations.find((installedMigration: DbMigrationsModel) => installedMigration.name === migration.name) != null;
                                                        const actionDisabled = (this.state.testConnectionResult !== TestConnectionResult.Success && this.state.testConnectionResult !== TestConnectionResult.PendingMigrations);

                                                        return {
                                                            title: migration.name,
                                                            subtitle: installed ? "Installed" : "Not installed",
                                                            enabled: !actionDisabled,
                                                            hidden: installed,
                                                            icon: mdiArrowUpBoldCircleOutline,
                                                            onExecute: () => this.onApplyMigration(migration.name)
                                                        };
                                                    })
                                                    } />
                                                )
                                                : (<Typography variant="body2">No migration found.</Typography>)
                                            }
                                        </div>

                                        <div hidden={this.state.activeTab !== 2}>
                                        {this.props.DataModel.setups.length !== 0
                                            ? (<ExecuterList items={
                                                this.props.DataModel.setups.map((setup, idx) => {
                                                    return {
                                                        title: setup.name,
                                                        subtitle: setup.description,
                                                        enabled: true,
                                                        hidden: false,
                                                        icon: mdiCogPlayOutline,
                                                        onExecute: () => this.onExecuteSetup(setup)
                                                    };
                                                })
                                                } />
                                            )
                                            : (<Typography variant="body2">No setup found.</Typography>)}
                                        </div>
                                    </GridLegacy>
                                </GridLegacy>
                            </GridLegacy>
                        </DatabaseSection>
                    </GridLegacy>
                </CardContent >
            </Card >
        );
    }
}

function contextNameWithoutNamespace(databaseModel: string): string {
    return databaseModel.replace(/^.+\./, "");
}

export default connect<{}, DatabaseModelDispatchPropsModel>(null, mapDispatchToProps)(DatabaseModel);
