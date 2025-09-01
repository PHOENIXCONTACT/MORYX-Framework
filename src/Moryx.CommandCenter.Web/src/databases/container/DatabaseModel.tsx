/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiArrowUpBoldCircleOutline, mdiCogPlayOutline, mdiConnection, mdiDatabaseAlert, mdiDatabaseCheckOutline, mdiTableAlert } from "@mdi/js";
import Button from "@mui/material/Button";
import ButtonGroup from "@mui/material/ButtonGroup";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import CircularProgress from "@mui/material/CircularProgress";
import Grid from "@mui/material/Grid";
import MenuItem from "@mui/material/MenuItem";
import Stack from "@mui/material/Stack";
import SvgIcon from "@mui/material/SvgIcon";
import Tab from "@mui/material/Tab";
import Tabs from "@mui/material/Tabs";
import TextField from "@mui/material/TextField";
import Tooltip from "@mui/material/Tooltip";
import Typography from "@mui/material/Typography";
import * as moment from "moment";
import * as React from "react";
import { connect } from "react-redux";
import { toast } from "react-toastify";
import kbToString from "../../common/converter/ByteConverter";
import { updateShowWaitDialog } from "../../common/redux/CommonActions";
import { ActionType } from "../../common/redux/Types";
import DatabasesRestClient from "../api/DatabasesRestClient";
import DatabaseConfigModel from "../models/DatabaseConfigModel";
import DatabaseConfigOptionModel from "../models/DatabaseConfigOptionModel";
import DataModel from "../models/DataModel";
import DbMigrationsModel from "../models/DbMigrationsModel";
import SetupModel from "../models/SetupModel";
import { TestConnectionResult } from "../models/TestConnectionResult";
import { updateDatabaseConfig } from "../redux/DatabaseActions";
import { DatabaseSection } from "./DatabaseSection";
import { ExecuterList } from "./ExecuterList";
import { getEnumValueForKey } from "../../modules/converter/EnumDecorator";

interface DatabaseModelPropsModel {
    RestClient: DatabasesRestClient;
    DataModel: DataModel;
}

interface DatabaseModelStateModel {
    activeTab: number;
    config: DatabaseConfigModel;
    targetModel: string;
    selectedBackup: string;
    testConnectionPending: boolean;
    testConnectionResult: TestConnectionResult;
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
            config: this.getConfigValue(),
            targetModel: props.DataModel.targetModel,
            selectedBackup : (props.DataModel.backups.length !== 0 ? props.DataModel.backups[0].fileName : ""),
            testConnectionPending : false,
            testConnectionResult : TestConnectionResult.ConfigurationError,
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
                targetModel: this.props.DataModel.targetModel
            });
            this.onTestConnection();
        }
    }

    public createConfigModel(): DatabaseConfigModel {
        return this.state.config;
    }

    public activeTab(event: React.SyntheticEvent, tabId: number): void {
        this.setState({ activeTab: tabId });
    }

    private getValidationState(entryName: string): React.JSX.ElementAttributesProperty {
        const result = this.props.DataModel.possibleConfigurators.find((x) => x.configuratorTypename === this.state.config.configuratorTypename)
            ?.properties.find((x) => x.name === entryName).required
                ? (this.state.config.entries[entryName]
                    ? { error: false }
                    : { error: true })
                : { error: false };

        return { props: result };
    }

    public onConfiguratorTypeChanged(e: React.ChangeEvent<HTMLInputElement>): void {
        const config = { ...this.state.config };
        config.configuratorTypename = e.target.value;
        config.entries = this.getConfigWithDefaultValue(e.target.value);
        this.setState({ config });
    }

    public onInputChanged(e: string, entryName: string): void {
        const config = {...this.state.config };
        config.entries[entryName] = e;
        this.setState({ config });
    }

    public onSelectBackup(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ selectedBackup: e.target.value });
    }

    private createEntriesInput(): React.JSX.Element[] {
        return Object.keys(this.state.config.entries)?.map((element) => {
            return (
                <TextField
                    key={element + "-input"}
                    label={element}
                    placeholder={element}
                    {...this.getValidationState(element)}
                    value={this.state.config.entries[element] ?? ""}
                    onBlur={() => this.onTestConnection()}
                    onChange={(e) => this.onInputChanged(e.target.value, element)}
                    variant="outlined"
                    size="small"
                    margin="dense"
                    />
            );
        });
    }

    private getConfigEntries(): any {
        const newEntries: any = {};
        this.props.DataModel.possibleConfigurators[0].properties.forEach((property) => {
            newEntries[property.name] = "";
        });
        return newEntries;
    }

    private getConfigWithDefaultValue(configuratorName: string): any {
        const newEntries: any = {};
        this.props.DataModel.possibleConfigurators
            .find((x) => x.configuratorTypename === configuratorName)
            .properties.forEach((property) => {
                const alternativeDefault = property.name.toLowerCase() === "database"
                    ? contextNameWithoutNamespace(this.state.targetModel)
                    : "";
                newEntries[property.name] = property.default ?? alternativeDefault;

            });
        return newEntries;
    }

    public getConfigValue(): DatabaseConfigModel {
        return {
            ...this.props.DataModel.config,
            entries: this.props.DataModel.config.entries ?
                this.props.DataModel.config.entries : this.getConfigEntries()
        };
    }

    public onSave(): void {
        this.props.onShowWaitDialog(true);

        this.onTestConnection();
        this.props.RestClient.saveDatabaseConfig(this.createConfigModel(), this.state.targetModel).then((response) => {
            this.props.onShowWaitDialog(false);

            this.setState({ config: response.config });
            this.props.onUpdateDatabaseConfig(response);
            toast.success("Configuration saved", { autoClose: 5000 });
            this.onTestConnection();
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onTestConnection(): void {
        this.setState({ testConnectionPending: true });
        this.props.RestClient.testDatabaseConfig(this.createConfigModel(), this.props.DataModel.targetModel)
            .then((response) => {
                this.setState({
                    testConnectionPending: false,
                    testConnectionResult: response.result !== undefined ? getEnumValueForKey(TestConnectionResult, response.result) : TestConnectionResult.ConnectionError
                });
            });
    }

    public onCreateDatabase(): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.createDatabase(this.createConfigModel(), this.props.DataModel.targetModel).then((data) => {
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

        this.props.RestClient.eraseDatabase(this.createConfigModel(), this.props.DataModel.targetModel).then((data) => {
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

    public onExecuteDump(): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.dumpDatabase(this.createConfigModel(), this.props.DataModel.targetModel).then((data) => {
            this.props.onShowWaitDialog(false);
            if (data.success) {
                toast.success("Database dump started successfully. Please refer to the log to get information about the progress.", { autoClose: 5000 });
            } else {
                toast.error("Dump start failed: " + data.errorMessage, { autoClose: 5000 });
            }
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onExecuteRestore(): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.restoreDatabase({ Config: this.createConfigModel(), BackupFileName: this.props.DataModel.targetModel }, this.state.targetModel).then((data) => {
            this.props.onShowWaitDialog(false);
            if (data.success) {
                toast.success("Database restore started successfully. Please refer to the log to get information about the progress.", { autoClose: 5000 });
            } else {
                toast.error("Restore start failed: " + data.errorMessage, { autoClose: 5000 });
            }
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onApplyMigration(migration: string): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.applyMigration(this.props.DataModel.targetModel, migration, this.createConfigModel()).then((data) => {
            this.props.onShowWaitDialog(false);

            if (data.wasUpdated) {
                this.props.RestClient.databaseModel(this.props.DataModel.targetModel).then((databaseConfig) => this.props.onUpdateDatabaseConfig(databaseConfig));
                toast.success("Migration applied", { autoClose: 5000 });
            } else {
                toast.error("Migration not applied", { autoClose: 5000 });
            }
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onRollbackDatabase(): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.rollbackDatabase(this.props.DataModel.targetModel, this.createConfigModel()).then((data) => {
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

        this.props.RestClient.executeSetup(this.props.DataModel.targetModel, { Config: this.createConfigModel(), Setup: setup }).then((data) => {
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
                    <Grid container={true} direction="column" spacing={1}>
                        <DatabaseSection title={(
                            <Typography
                                variant="h5"
                                gutterBottom={true}
                            >
                                {contextNameWithoutNamespace(this.state.targetModel)} {this.state.testConnectionPending
                                    ? (<CircularProgress size="small"/>)
                                    : this.preRenderConnectionCheckIcon()}
                            </Typography>
                            )}
                        >
                            <Grid item={true} md={12}>
                                <Stack spacing={1}>
                                    <TextField
                                        label="Configurator"
                                        onChange={(e: React.ChangeEvent<HTMLInputElement>) => this.onConfiguratorTypeChanged(e)}
                                        value={reviseConfiguratorName(this.state.config.configuratorTypename, this.props.DataModel.possibleConfigurators)}
                                        onBlur={() => this.onTestConnection()}
                                        variant="outlined"
                                        select={true}
                                        size="small">
                                            {this.props.DataModel.possibleConfigurators.map((config, idx) => (
                                                <MenuItem key={idx} value={config.configuratorTypename}>{config.name}</MenuItem>))
                                            }
                                    </TextField>
                                    {this.state.config.configuratorTypename && this.createEntriesInput()}
                                </Stack>
                            </Grid>
                            <Grid item={true} md={12}>
                                <Button color="primary" onClick={() => this.onSave()}>Save</Button>
                            </Grid>
                        </DatabaseSection>
                        <DatabaseSection title="Backup &amp; Restore">
                            <Grid item={true} md={12}>
                                <Stack>
                                    <TextField
                                        label="Backup"
                                        size="small"
                                        margin="dense"
                                        disabled={this.props.DataModel.backups.length === 0 }
                                        variant="outlined"
                                        select={true}
                                        onChange={(e: React.ChangeEvent<HTMLInputElement>) => this.onSelectBackup(e)}>
                                        {
                                            this.props.DataModel.backups.map((backup, idx) => {
                                                return (<MenuItem key={idx} value={backup.fileName}>{backup.fileName + " (Size: " + kbToString(backup.size * 1024) + ", Created on: " + moment(backup.creationDate).format("YYYY-MM-DD HH:mm:ss") + ")"}</MenuItem>);
                                            })
                                        }
                                    </TextField>
                                </Stack>
                            </Grid>
                            <Grid item={true} md={12}>
                                <ButtonGroup>
                                    <Button color="primary"
                                        disabled={this.state.testConnectionResult !== TestConnectionResult.Success && this.state.testConnectionResult !== TestConnectionResult.PendingMigrations}
                                        onClick={this.onExecuteDump.bind(this)}>
                                        Create a backup
                                    </Button>
                                    <Button color="primary"
                                        disabled={this.state.selectedBackup === "" || (this.state.testConnectionResult !== TestConnectionResult.Success && this.state.testConnectionResult !== TestConnectionResult.PendingMigrations)}
                                        onClick={this.onExecuteRestore.bind(this)}>
                                        Restore selected backup
                                    </Button>
                                </ButtonGroup>
                            </Grid>
                        </DatabaseSection>
                        <DatabaseSection title="Database">
                            <Grid item={true} md={12}>
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
                            </Grid>
                            <Grid container={true} item={true}
                                md={12} spacing={1}
                                direction="row"

                            >
                                <Grid item={true} md={12}>
                                    <Tabs value={this.state.activeTab} onChange={this.activeTab}>
                                        <Tab label="Migrations" value={1} />
                                        <Tab label="Setups" value={2} />
                                    </Tabs>
                                </Grid>

                                <Grid container={true} item={true} md={12}
                                    direction="column"
                                    justifyContent="flex-start"
                                    alignItems="stretch"
                                >
                                    <Grid item={true}>
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
                                    </Grid>
                                </Grid>
                            </Grid>
                        </DatabaseSection>
                    </Grid>
                </CardContent >
            </Card >
        );
    }
}

function contextNameWithoutNamespace(databaseModel: string): string {
    return databaseModel.replace(/^.+\./, "");
}

function reviseConfiguratorName(name: string, possibleConfigurators: DatabaseConfigOptionModel[]): string {
    const result = name;

    let configurator = possibleConfigurators.find((pc) => pc.configuratorTypename === result);

    if (configurator != null) {
        return result;
    }

    configurator = possibleConfigurators.find((pc) => pc.configuratorTypename.startsWith(result.replace(/,.+/, "")));

    return configurator != null
        ? configurator.configuratorTypename
        : "";
}

export default connect<{}, DatabaseModelDispatchPropsModel>(null, mapDispatchToProps)(DatabaseModel);
