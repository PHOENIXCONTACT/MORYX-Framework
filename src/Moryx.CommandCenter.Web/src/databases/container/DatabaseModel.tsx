/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiBriefcase, mdiCheck, mdiDatabase, mdiExclamationThick, mdiLoading, mdiPowerPlug, mdiTable} from "@mdi/js";
import Icon from "@mdi/react";
import * as moment from "moment";
import { string } from "prop-types";
import * as React from "react";
import NotificationSystem = require("react-notification-system");
import { connect, Provider } from "react-redux";
import { Button, ButtonGroup, Card, CardBody, CardHeader, Col, Container, Form, Input, Nav, NavItem, NavLink, Row, TabContent, TabPane, UncontrolledTooltip } from "reactstrap";
import kbToString from "../../common/converter/ByteConverter";
import { updateShowWaitDialog } from "../../common/redux/CommonActions";
import { ActionType } from "../../common/redux/Types";
import "../../common/scss/Theme.scss";
import NavigableConfigEditor from "../../modules/components/ConfigEditor/NavigableConfigEditor";
import Entry from "../../modules/models/Entry";
import DatabasesRestClient from "../api/DatabasesRestClient";
import NavigableEntry from "../components/SettingEditor/NavigableEntry";
import DatabaseConfigModel from "../models/DatabaseConfigModel";
import DataModel from "../models/DataModel";
import DbMigrationsModel from "../models/DbMigrationsModel";
import { TestConnectionResult } from "../models/TestConnectionResult";
import { updateDatabaseConfig } from "../redux/DatabaseActions";

interface DatabaseModelPropsModel {
    RestClient: DatabasesRestClient;
    DataModel: DataModel;
    NotificationSystem: NotificationSystem;
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

interface DatabaseModelStateModel {
    activeTab: string;
    config: Entry;
    connectionString: string;
    configuratorTypename: string;
    selectedMigration: string;
    selectedSetup: number;
    selectedBackup: string;
    testConnectionPending: boolean;
    testConnectionResult: TestConnectionResult;
}

class DatabaseModel extends React.Component<DatabaseModelPropsModel & DatabaseModelDispatchPropsModel, DatabaseModelStateModel> {
    private databaseConfiguratorTypes: string[] = [];
    constructor(props: DatabaseModelPropsModel & DatabaseModelDispatchPropsModel) {
        super(props);
        this.state = {
            activeTab: "1",
            config : this.props.DataModel.config,
            configuratorTypename: this.getCurrentConfiguratorTypeName(this.props.DataModel.config),
            connectionString: this.getConnectionSettings(this.props.DataModel.config).connectionString,
            selectedMigration: (this.props.DataModel.availableMigrations.length !== 0 ? this.props.DataModel.availableMigrations[0].name : ""),
            selectedSetup : (this.props.DataModel.setups.length !== 0 ? 0 : -1),
            selectedBackup : (this.props.DataModel.backups.length !== 0 ? this.props.DataModel.backups[0].fileName : ""),
            testConnectionPending: false,
            testConnectionResult: TestConnectionResult.ConfigurationError,
        };
        this.setConfiguratorTypeNames(this.props.DataModel.config);
    }

    public componentDidMount(): void {
        this.onTestConnection();

    }

    public getConnectionSettings(configEntry: Entry) {
        return {
            database : configEntry.subEntries.find((x) => x.displayName === "ConnectionSettings")
            .subEntries.find((n) => n.displayName === "Database").value.current,
            connectionString: configEntry.subEntries.find((x) => x.displayName === "ConnectionSettings")
            .subEntries.find((n) => n.displayName === "ConnectionString").value.current
        };
    }

    public getConnectionSettingsEntry(configEntry: Entry): Entry {
        return configEntry.subEntries.find((x) => x.displayName === "ConnectionSettings");
    }

    public setConfiguratorTypeNames(configEntry: Entry) {
        return this.databaseConfiguratorTypes = configEntry.subEntries.find((x) => x.displayName === "ConfiguratorTypename")
        .value.possible;
    }

    public getCurrentConfiguratorTypeName(configEntry: Entry) {
        return  configEntry.subEntries.find((x) => x.displayName === "ConfiguratorTypename")
        .value.current;
    }

    public getCurrentConfiguratorTypeNameEntry(configEntry: Entry): Entry {
        return  configEntry.subEntries.find((x) => x.displayName === "ConfiguratorTypename");
    }

    public createConfigModel(): DatabaseConfigModel {
        const connectionSettingsEntry = this.getConnectionSettingsEntry(this.state.config);
        connectionSettingsEntry.subEntries = connectionSettingsEntry.subEntries.map((connectionSettingsSubentries) => {
            if (connectionSettingsSubentries.displayName === "ConnectionString") {
                 connectionSettingsEntry.value.current = this.state.connectionString;
            }

            return connectionSettingsSubentries;
        });

        const configuratorTypenameEntry = this.getCurrentConfiguratorTypeNameEntry(this.state.config);
        configuratorTypenameEntry.value.current = this.state.configuratorTypename;

        const newConfig = this.state.config;
        newConfig.subEntries = newConfig.subEntries.map((subEntry) => {
            if (subEntry.displayName === "ConnectionString") {
                subEntry = connectionSettingsEntry;
            } else if (subEntry.displayName === "ConfiguratorTypename") {
                subEntry = configuratorTypenameEntry;
 }

            return subEntry;
        });

        this.setState({config : newConfig});
        console.log("new state =", this.state.config);
        return {
        config : newConfig,
       };
    }

    // Public buildConnectionString() {
    //     Const dbConfiguratorName = this.state.configuratorTypename ?? this.databaseConfiguratorTypes[0].typeName;
    //     Const dbConfigurator = this.databaseConfiguratorTypes.find((provider) => provider.typeName === dbConfiguratorName) ?? this.databaseConfiguratorTypes[0];
    //     Let template = dbConfigurator.defaultConnectionString;
    //     Object.keys(this.connectionStringFormatter).forEach((key) => {
    //         Template = template.replace(`<${key}>`, this.connectionStringFormatter[key]);
    //     });
    //     Return template;
    // }

    // Public buildEncryptedConnectionString(configuratorName: string) {
    //     Const dbConfigurator = this.databaseConfiguratorTypes.find((configurator) => configurator.typeName === configuratorName);
    //     If (!dbConfigurator) {  return ""; }
    //     Let template = dbConfigurator.defaultConnectionString;
    //     Object.keys(this.connectionStringFormatter).forEach((key) => {
    //         If (key != "password") {
    //         Template = template.replace(`<${key}>`, this.connectionStringFormatter[key]);
    //         } else {
    //         // Replace the password with hidden string
    //         Template = template.replace(`<${key}>`, this.getHiddenPassword(this.connectionStringFormatter[key]));
    //         }
    //     });
    //     Return template;
    // }

    // Public buildConnectionString() {
    //     Const dbConfiguratorName = this.state.configuratorTypename ?? this.databaseConfiguratorTypes[0].typeName;
    //     Const dbConfigurator = this.databaseConfiguratorTypes.find((provider) => provider.typeName === dbConfiguratorName) ?? this.databaseConfiguratorTypes[0];
    //     Let template = dbConfigurator.defaultConnectionString;
    //     Object.keys(this.connectionStringFormatter).forEach((key) => {
    //         Template = template.replace(`<${key}>`, this.connectionStringFormatter[key]);
    //     });
    //     Return template;
    // }

    // Public buildEncryptedConnectionString(configuratorName: string) {
    //     Const dbConfigurator = this.databaseConfiguratorTypes.find((configurator) => configurator.typeName === configuratorName);
    //     If (!dbConfigurator) {  return ""; }
    //     Let template = dbConfigurator.defaultConnectionString;
    //     Object.keys(this.connectionStringFormatter).forEach((key) => {
    //         If (key != "password") {
    //         Template = template.replace(`<${key}>`, this.connectionStringFormatter[key]);
    //         } else {
    //         // Replace the password with hidden string
    //         Template = template.replace(`<${key}>`, this.getHiddenPassword(this.connectionStringFormatter[key]));
    //         }
    //     });
    //     Return template;
    // }

    // Public buildConnectionString() {
    //     Const dbConfiguratorName = this.state.configuratorTypename ?? this.databaseConfiguratorTypes[0].typeName;
    //     Const dbConfigurator = this.databaseConfiguratorTypes.find((provider) => provider.typeName === dbConfiguratorName) ?? this.databaseConfiguratorTypes[0];
    //     Let template = dbConfigurator.defaultConnectionString;
    //     Object.keys(this.connectionStringFormatter).forEach((key) => {
    //         Template = template.replace(`<${key}>`, this.connectionStringFormatter[key]);
    //     });
    //     Return template;
    // }

    // Public buildEncryptedConnectionString(configuratorName: string) {
    //     Const dbConfigurator = this.databaseConfiguratorTypes.find((configurator) => configurator.typeName === configuratorName);
    //     If (!dbConfigurator) {  return ""; }
    //     Let template = dbConfigurator.defaultConnectionString;
    //     Object.keys(this.connectionStringFormatter).forEach((key) => {
    //         If (key != "password") {
    //         Template = template.replace(`<${key}>`, this.connectionStringFormatter[key]);
    //         } else {
    //         // Replace the password with hidden string
    //         Template = template.replace(`<${key}>`, this.getHiddenPassword(this.connectionStringFormatter[key]));
    //         }
    //     });
    //     Return template;
    // }

    // Public getHiddenPassword(value: string) {
    //     Let result = "";
    //     For (let i = 0; i < value.length; i++) {
    //     Result += "*";
    //     }

    //     Return result;
    // }

    public activeTab(tabId: string): void {
        this.setState({activeTab: tabId});
    }

    public onSelectMigration(e: React.FormEvent<HTMLInputElement>): void {
        this.setState({selectedMigration: (e.target as HTMLSelectElement).value});
    }

    public onSelectSetup(e: React.FormEvent<HTMLInputElement>): void {
        this.setState({selectedSetup: (e.target as HTMLSelectElement).selectedIndex});
    }

    public onSelectBackup(e: React.FormEvent<HTMLInputElement>): void {
        this.setState({selectedBackup: (e.target as HTMLSelectElement).value});
    }

    public onChangeHost(e: React.FormEvent<HTMLInputElement>): void {
       // This.setState({host: (e.target as HTMLInputElement).value});
    }

    public onChangePort(e: React.FormEvent<HTMLInputElement>): void {
       // This.setState({port: parseInt((e.target as HTMLInputElement).value, 10)});
    }

    public onChangeDatabase(e: React.FormEvent<HTMLInputElement>): void {
       // This.setState({database: (e.target as HTMLInputElement).value});
    }

    public onChangeUsername(e: React.FormEvent<HTMLInputElement>): void {
       // This.setState({username: (e.target as HTMLInputElement).value});
    }

    public onChangePassword(e: React.FormEvent<HTMLInputElement>): void {
       // This.setState({password: (e.target as HTMLInputElement).value});
    }

    public onChangeConnectionString(e: React.FormEvent<HTMLInputElement>): void {
       this.setState({connectionString: (e.target as HTMLInputElement).value});
    }

    public onChangeProvider(e: React.FormEvent<HTMLInputElement>): void {
       // Console.log("new configurator :", (e.target as HTMLInputElement).value);
        this.setState({configuratorTypename: (e.target as HTMLInputElement).value});
    }

    public onSave(): void {
        this.props.onShowWaitDialog(true);

        // Replace the encrypted connectionString with the normal connection string
        this.props.RestClient.saveDatabaseConfig(this.createConfigModel(), this.props.DataModel.targetModel).then(() => {
            this.props.onShowWaitDialog(false);

            this.props.RestClient.databaseModel(this.props.DataModel.targetModel).then((data) => this.props.onUpdateDatabaseConfig(data));
            this.props.NotificationSystem.addNotification({ title: "Configuration saved", message: "", level: "success", autoDismiss: 5 });
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onTestConnection(): void {
        this.setState({ testConnectionPending: true });
        this.props.RestClient.testDatabaseConfig(this.createConfigModel(), this.props.DataModel.targetModel)
                             .then((response) => this.setState({ testConnectionPending: false,
                                                                 testConnectionResult: response.result,
                     }));
    }

    public onCreateDatabase(): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.createDatabase(this.createConfigModel(), this.props.DataModel.targetModel).then((data) => {
            this.props.onShowWaitDialog(false);

            if (data.success) {
                this.props.RestClient.databaseModel(this.props.DataModel.targetModel).then((databaseConfig) => this.props.onUpdateDatabaseConfig(databaseConfig));
                this.props.NotificationSystem.addNotification({ title: "Success", message: "Database created successfully", level: "success", autoDismiss: 5 });
           } else {
                this.props.NotificationSystem.addNotification({ title: "Error", message: "Database not created: " + data.errorMessage, level: "error", autoDismiss: 5 });
           }
            this.onTestConnection();
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onEraseDatabase(): void {
        if (confirm("Do you really want to delete the entire database?") == false) {
            return;
        }

        this.props.onShowWaitDialog(true);

        this.props.RestClient.eraseDatabase(this.createConfigModel(), this.props.DataModel.targetModel).then((data) => {
            this.props.onShowWaitDialog(false);

            if (data.success) {
                this.props.RestClient.databaseModel(this.props.DataModel.targetModel).then((databaseConfig) => this.props.onUpdateDatabaseConfig(databaseConfig));
                this.props.NotificationSystem.addNotification({ title: "Success", message: "Database deleted successfully", level: "success", autoDismiss: 5 });
            } else {
                this.props.NotificationSystem.addNotification({ title: "Error", message: "Database not deleted: " + data.errorMessage, level: "error", autoDismiss: 5 });
            }
            this.onTestConnection();
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onExecuteDump(): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.dumpDatabase(this.createConfigModel(), this.props.DataModel.targetModel).then((data) => {
            this.props.onShowWaitDialog(false);

            if (data.success) {
                this.props.NotificationSystem.addNotification({ title: "Success", message: "Database dump started successfully. Please refer to the log to get information about the progress.", level: "success", autoDismiss: 5 });
            } else {
                this.props.NotificationSystem.addNotification({ title: "Error", message: "Dump start failed: " + data.errorMessage, level: "error", autoDismiss: 5 });
            }
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onExecuteRestore(): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.restoreDatabase({ Config: this.createConfigModel(), BackupFileName: this.state.selectedBackup }, this.props.DataModel.targetModel).then((data) => {
            this.props.onShowWaitDialog(false);

            if (data.success) {
                this.props.NotificationSystem.addNotification({ title: "Success", message: "Database restore started successfully. Please refer to the log to get information about the progress.", level: "success", autoDismiss: 5 });
            } else {
                this.props.NotificationSystem.addNotification({ title: "Error", message: "Restore start failed: " + data.errorMessage, level: "error", autoDismiss: 5 });
            }
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onApplyMigration(): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.applyMigration(this.props.DataModel.targetModel, this.state.selectedMigration, this.createConfigModel()).then((data) => {
            this.props.onShowWaitDialog(false);

            if (data.wasUpdated) {
                this.props.RestClient.databaseModel(this.props.DataModel.targetModel).then((databaseConfig) => this.props.onUpdateDatabaseConfig(databaseConfig));
                this.props.NotificationSystem.addNotification({ title: "Success", message: "Migration applied", level: "success", autoDismiss: 5 });
            } else {
                this.props.NotificationSystem.addNotification({ title: "Error", message: "Migration not applied", level: "error", autoDismiss: 5 });
            }
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onRollbackDatabase(): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.rollbackDatabase(this.props.DataModel.targetModel, this.createConfigModel()).then((data) => {
            this.props.onShowWaitDialog(false);

            if (data.success) {
                this.props.RestClient.databaseModel(this.props.DataModel.targetModel).then((databaseConfig) => this.props.onUpdateDatabaseConfig(databaseConfig));
                this.props.NotificationSystem.addNotification({ title: "Success", message: "Database rollback completed successfully", level: "success", autoDismiss: 5 });
            } else {
                this.props.NotificationSystem.addNotification({ title: "Error", message: "Database rollback failed: " + data.errorMessage, level: "error", autoDismiss: 5 });
            }
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onExecuteSetup(): void {
        this.props.onShowWaitDialog(true);

        const foundSetup = this.props.DataModel.setups[this.state.selectedSetup];

        this.props.RestClient.executeSetup(this.props.DataModel.targetModel, { Config: this.createConfigModel(), Setup: foundSetup }).then((data) => {
            this.props.onShowWaitDialog(false);

            if (data.success) {
                this.props.NotificationSystem.addNotification({ title: "Success", message: "Setup '" + foundSetup.name + "' executed successfully", level: "success", autoDismiss: 5 });
            } else {
                this.props.NotificationSystem.addNotification({ title: "Error", message: data.errorMessage, level: "error", autoDismiss: 5 });
            }
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    private preRenderConnectionCheckIcon(): React.ReactNode {
        switch (this.state.testConnectionResult) {
            case TestConnectionResult.Success:
                return (<Icon path={mdiCheck} className="icon-green" />);
            case TestConnectionResult.ConfigurationError:
                return (<div style={{display: "inline", color: "red"}} id="TestConnectionErrorHint">
                            <Icon path={mdiExclamationThick} className="icon-red right-space" />
                            <Icon path={mdiTable} className="icon-red"/>
                            <UncontrolledTooltip placement="right" target="TestConnectionErrorHint">
                                Please check if model configuration exists on server.
                            </UncontrolledTooltip>
                        </div>);
            case TestConnectionResult.ConnectionError:
                return (<div style={{display: "inline", color: "red"}} id="TestConnectionErrorHint">
                            <Icon path={mdiExclamationThick} className="icon-red right-space" />
                            <Icon path={mdiPowerPlug} className="icon-red"/>
                            <UncontrolledTooltip placement="right" target="TestConnectionErrorHint">
                                Please check host, port and credentials.
                            </UncontrolledTooltip>
                        </div>);
            case TestConnectionResult.ConnectionOkDbDoesNotExist:
                return (<div style={{display: "inline", color: "red"}} id="TestConnectionErrorHint">
                        <Icon path={mdiExclamationThick} className="icon-red right-space" />
                        <Icon path={mdiDatabase} className="icon-red"/>
                        <UncontrolledTooltip placement="right" target="TestConnectionErrorHint">
                            The connection to the database could be established but the database could not be found. Please check the name of the database or create it before.
                        </UncontrolledTooltip>
                    </div>);
            default:
                return (<div />);
        }
    }

    public render(): React.ReactNode {
        return (
            <Card>
                <CardHeader tag="h2">
                    <Icon path={mdiBriefcase} className="icon right-space" />
                    {this.props.DataModel.targetModel}
                </CardHeader>
                <CardBody>
                    <Container fluid={true}>
                        <Row>
                            <Col md={6}>
                                <h3>
                                    <span className="right-space">Connection</span>
                                    { this.state.testConnectionPending ? (
                                        <Icon path={mdiLoading} spin={true} className="icon"/>
                                    ) : this.preRenderConnectionCheckIcon() }
                                </h3>
                            </Col>
                            <Col md={6}>
                                <h3>Backup &amp; Restore</h3>
                            </Col>
                        </Row>
                        <Row>
                            <Col md={6}>
                                <Container fluid={true}>
                                    <Row >
                                        <Col md={12}>
                                            <Input type="select"  placeholder={"Database provider"}
                                                value={this.state.configuratorTypename}
                                                onChange={(e: React.FormEvent<HTMLInputElement>) => this.onChangeProvider(e)}
                                                onBlur={this.onTestConnection.bind(this)}
                                             >
                                                {
                                                    this.databaseConfiguratorTypes.map((provider, idx) => {
                                                        return (<option key={idx} value={provider}>{provider}</option>);
                                                    })
                                                }
                                            </Input>
                                        </Col>
                                    </Row>
                                    {/* <Row className="up-space">
                                        <Col md={12}>
                                            <Form inline={true}>
                                                <Input placeholder={"Host"} value={this.state.host} onChange={(e: React.FormEvent<HTMLInputElement>) => this.onChangeHost(e)} onBlur={this.onTestConnection.bind(this)} style={{width: "78%"}} />
                                                <span className="center-text" style={{width: "2%"}}>:</span>
                                                <Input placeholder={"Port"} value={this.state.port} onChange={(e: React.FormEvent<HTMLInputElement>) => this.onChangePort(e)} onBlur={this.onTestConnection.bind(this)} style={{width: "20%"}} />
                                            </Form>
                                        </Col>
                                    </Row> */}
                                    {/* <Row className="up-space">
                                        <Col md={12}><Input placeholder={"Name of database"} value={this.state.database} onChange={(e: React.FormEvent<HTMLInputElement>) => this.onChangeDatabase(e)} onBlur={this.onTestConnection.bind(this)} /></Col>
                                    </Row>
                                    <Row className="up-space">
                                        <Col md={12}><Input placeholder={"Username"} value={this.state.username} onChange={(e: React.FormEvent<HTMLInputElement>) => this.onChangeUsername(e)} onBlur={this.onTestConnection.bind(this)} /></Col>
                                    </Row>
                                    <Row className="up-space">
                                        <Col md={12}><Input type="password" placeholder={"Password"} value={this.state.password} onChange={(e: React.FormEvent<HTMLInputElement>) => this.onChangePassword(e)} onBlur={this.onTestConnection.bind(this)} /></Col>
                                    </Row> */}
                                   <Row className="up-space">
                                        <Col md={12}><Input className="lg-height" type="textarea" placeholder={"connectionString"} value={this.state.connectionString} onChange={(e: React.FormEvent<HTMLInputElement>) => this.onChangeConnectionString(e)} onBlur={this.onTestConnection.bind(this)} /></Col>
                                    </Row>
                                    <Row className="up-space-lg">
                                        <Col md={12}>
                                            <Button color="primary" onClick={() => this.onSave()}>Save</Button>
                                        </Col>
                                    </Row>
                                </Container>
                            </Col>
                            <Col md={6}>
                                <Container fluid={true}>
                                    <Row>
                                        <Col md={12}>
                                            <Input type="select" size={5} className="auto-height"
                                                onChange={(e: React.FormEvent<HTMLInputElement>) => this.onSelectBackup(e)}>
                                            {
                                                this.props.DataModel.backups.map((backup, idx) => {
                                                    return (<option key={idx} value={backup.fileName}>{backup.fileName + " (Size: " + kbToString(backup.size * 1024) + ", Created on: " + moment(backup.creationDate).format("YYYY-MM-DD HH:mm:ss") + ")"}</option>);
                                                })
                                            }
                                            </Input>
                                        </Col>
                                    </Row>
                                    <Row className="up-space-lg">
                                        <Col md={12}>
                                            <ButtonGroup>
                                                <Button color="primary"
                                                        disabled={this.state.testConnectionResult !== TestConnectionResult.Success}
                                                        onClick={this.onExecuteDump.bind(this)}>
                                                    Create a backup
                                                </Button>
                                                <Button color="primary"
                                                        disabled={this.state.selectedBackup === "" || this.state.testConnectionResult !== TestConnectionResult.Success}
                                                        onClick={this.onExecuteRestore.bind(this)}>
                                                    Restore selected backup
                                                </Button>
                                            </ButtonGroup>
                                        </Col>
                                    </Row>
                                </Container>
                            </Col>
                        </Row>
                        <Row className="up-space-lg">
                            <Col md={6}>
                                <Container fluid={true}>
                                        <Row>
                                            <h3>Database</h3>
                                        </Row>
                                        <Row>
                                            <Col md={12}>
                                                <ButtonGroup>
                                                    <Button color="primary"
                                                            onClick={() => this.onCreateDatabase()}
                                                            disabled={this.state.testConnectionResult !== TestConnectionResult.ConnectionOkDbDoesNotExist}>
                                                        Create database
                                                    </Button>
                                                    <Button color="primary"
                                                            onClick={() => this.onEraseDatabase()}
                                                            disabled={this.state.testConnectionResult !== TestConnectionResult.Success}>
                                                        Erase database
                                                    </Button>
                                                </ButtonGroup>
                                            </Col>
                                        </Row>
                                </Container>
                            </Col>
                        </Row>
                        <Row className="up-space-lg">
                            <Col md={12}>
                                <Nav tabs={true}>
                                    <NavItem>
                                        <NavLink active={this.state.activeTab === "1"} className={"selectable"} onClick={() => { this.activeTab("1"); }}>Migrations</NavLink>
                                    </NavItem>
                                    <NavItem>
                                        <NavLink active={this.state.activeTab === "2"} className={"selectable"} onClick={() => { this.activeTab("2"); }}>Setups</NavLink>
                                    </NavItem>
                                </Nav>
                                <TabContent activeTab={this.state.activeTab}>
                                    <TabPane tabId="1">
                                        { this.props.DataModel.availableMigrations.length !== 0 ?
                                            (
                                            <Container fluid={true}>
                                                <Row>
                                                    <Col md={12}>
                                                        <Input type="select" size={10} className="auto-height"
                                                            onChange={(e: React.FormEvent<HTMLInputElement>) => this.onSelectMigration(e)}>
                                                        {
                                                            this.props.DataModel.availableMigrations.map((migration, idx) => {
                                                                const installed = this.props.DataModel.appliedMigrations.find((installedMigration: DbMigrationsModel) => installedMigration.name === migration.name);
                                                                const option = migration.name + " (" + (installed ? "Installed" : "Not installed") + ")";

                                                                return (<option key={idx} value={migration.name}>{option}</option>);
                                                            })
                                                        }
                                                        </Input>
                                                    </Col>
                                                </Row>
                                                <Row className="up-space-lg">
                                                    <Col md={12}>
                                                        <ButtonGroup>
                                                            <Button color="primary"
                                                                    onClick={() => this.onApplyMigration()}
                                                                    disabled={this.state.selectedMigration === "" || this.state.testConnectionResult !== TestConnectionResult.Success}>
                                                                Apply selected migration
                                                            </Button>
                                                            <Button color="primary"
                                                                    onClick={() => this.onRollbackDatabase()}
                                                                    disabled={this.props.DataModel.availableMigrations.length === 0 || this.state.testConnectionResult !== TestConnectionResult.Success}>
                                                                Rollback all migrations
                                                            </Button>
                                                        </ButtonGroup>
                                                    </Col>
                                                </Row>
                                            </Container>
                                            ) : (
                                                <Row>
                                                    <Col>
                                                        <span className="font-italic">No migrations found.</span>
                                                    </Col>
                                                </Row>
                                            )}
                                    </TabPane>
                                    <TabPane tabId="2">
                                        { this.props.DataModel.setups.length !== 0 ?
                                            (
                                            <Container fluid={true}>
                                                <Row>
                                                    <Col md={12}>
                                                        <Input type="select" size={10} className="auto-height"
                                                            onChange={(e: React.FormEvent<HTMLInputElement>) => this.onSelectSetup(e)}>
                                                        {
                                                            this.props.DataModel.setups.map((setup, idx) => {
                                                                return (<option key={idx} value={setup.name}>{setup.name} - {setup.description}</option>);
                                                            })
                                                        }
                                                        </Input>
                                                    </Col>
                                                </Row>
                                                <Row className="up-space-lg">
                                                    <Col md={12}>
                                                        <Button color="primary"
                                                                onClick={() => this.onExecuteSetup()}
                                                                disabled={this.state.selectedSetup === -1 || this.state.testConnectionResult !== TestConnectionResult.Success}>
                                                            Execute setup
                                                        </Button>
                                                    </Col>
                                                </Row>
                                            </Container>
                                            ) : (
                                                <Row>
                                                    <Col>
                                                        <span className="font-italic">No setups found.</span>
                                                    </Col>
                                                </Row>
                                            )}
                                    </TabPane>
                                </TabContent>
                            </Col>
                        </Row>
                    </Container>
                </CardBody>
            </Card>
        );
    }
}

export default connect<{}, DatabaseModelDispatchPropsModel>(null, mapDispatchToProps)(DatabaseModel);
