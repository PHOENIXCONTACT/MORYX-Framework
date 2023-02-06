/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiBriefcase, mdiCheck, mdiDatabase, mdiExclamationThick, mdiLoading, mdiPowerPlug, mdiTable} from "@mdi/js";
import Icon from "@mdi/react";
import * as moment from "moment";
import { any, element, string } from "prop-types";
import * as React from "react";
import NotificationSystem = require("react-notification-system");
import { connect, Provider } from "react-redux";
import { RouteComponentProps, withRouter } from "react-router-dom";
import { Button, ButtonGroup, Card, CardBody, CardHeader, Col, Container, Form, Input, Nav, NavItem, NavLink, Row, TabContent, TabPane, UncontrolledTooltip } from "reactstrap";
import kbToString from "../../common/converter/ByteConverter";
import { updateShowWaitDialog } from "../../common/redux/CommonActions";
import { ActionType } from "../../common/redux/Types";
import "../../common/scss/Theme.scss";
import DatabasesRestClient from "../api/DatabasesRestClient";
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

interface DatabaseModelStateModel {
    activeTab: string;
    config: DatabaseConfigModel;
    selectedMigration: string;
    selectedSetup: number;
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
            activeTab: "1",
            config : this.getConfigValue(),
            selectedMigration: (this.props.DataModel.availableMigrations.length !== 0 ? this.props.DataModel.availableMigrations[0].name : ""),
            selectedSetup : (this.props.DataModel.setups.length !== 0 ? 0 : -1),
            selectedBackup : (this.props.DataModel.backups.length !== 0 ? this.props.DataModel.backups[0].fileName : ""),
            testConnectionPending: false,
            testConnectionResult: TestConnectionResult.ConfigurationError,
        };
    }

    public componentDidMount(): void {
        this.onTestConnection();
    }

    public createConfigModel(): DatabaseConfigModel {
        return this.state.config;
    }

    public activeTab(tabId: string): void {
        this.setState({activeTab: tabId});
    }

    public onSelectMigration(e: React.FormEvent<HTMLInputElement>): void {
        this.setState({selectedMigration: (e.target as HTMLSelectElement).value});
    }

    public onSelectSetup(e: React.FormEvent<HTMLInputElement>): void {
        this.setState({selectedSetup: (e.target as HTMLSelectElement).selectedIndex});
    }

    public getValidationState(entryName: string) {
        const result = this.props.DataModel.possibleConfigurators.find((x) => x.configuratorTypename === this.state.config.configuratorTypename)
        ?.properties.find((x) => x.name === entryName).required ?
        (this.state.config.entries[entryName] ? {valid : true, invalid : false} : {invalid : true , valid : false}) : {valid : true, invalid : false};

        return result;
    }

    public onConfiguratorTypeChanged(e: React.FormEvent<HTMLInputElement>): void {
        this.setState({config: {...this.state.config, configuratorTypename: (e.target as HTMLSelectElement).value ,
                                entries: this.getConfigWithDefaultValue((e.target as HTMLSelectElement).value)}});
    }

    public onInputChanged(e: React.FormEvent<HTMLInputElement>, entryName: string): void {
        this.setState({
            config : {...this.state.config, entries : {...this.state.config.entries, [entryName]: (e.target as HTMLSelectElement).value}}
        });
    }

    public onSelectBackup(e: React.FormEvent<HTMLInputElement>): void {
        this.setState({selectedBackup: (e.target as HTMLSelectElement).value});
    }

    public createEntriesInput() {
        return Object.keys(this.state.config.entries)?.map((element) => {
            return (<Col md={12} className="up-space">
                            <Input placeholder={element} {...this.getValidationState(element)} value={this.state.config.entries[element]} onBlur={() => this.onTestConnection()} onChange={(e: React.FormEvent<HTMLInputElement>) => this.onInputChanged(e, element)}/>
                    </Col>);
            });
    }

    public getConfigEntries() {
        const newEntries: any = {};
        this.props.DataModel.possibleConfigurators[0].properties.forEach((property) => {
            newEntries[property.name] = "";
        });
        return newEntries;
    }

    public getConfigWithDefaultValue(configurator: string) {
        const newEntries: any = {};
        this.props.DataModel.possibleConfigurators.find((x) => x.configuratorTypename === configurator).properties.forEach((property) => {
            newEntries[property.name] = property.default ?? "";
        });
        return newEntries;
    }

    public getConfigValue() {
        return {...this.props.DataModel.config,
                entries: this.props.DataModel.config.entries ?
            this.props.DataModel.config.entries : this.getConfigEntries()
        };
    }

    public onSave(): void {
        this.props.onShowWaitDialog(true);

        this.onTestConnection();
        this.props.RestClient.saveDatabaseConfig(this.createConfigModel(), this.props.DataModel.targetModel).then((response) => {
            this.props.onShowWaitDialog(false);

            this.setState({config: response.config});
            this.props.onUpdateDatabaseConfig(response);
            this.props.NotificationSystem.addNotification({ title: "Configuration saved", message: "", level: "success", autoDismiss: 5 });
            this.onTestConnection();
        }).catch((d) => this.props.onShowWaitDialog(false));
    }

    public onTestConnection(): void {
        this.setState({ testConnectionPending: true });
        this.props.RestClient.testDatabaseConfig(this.createConfigModel(), this.props.DataModel.targetModel)
                             .then((response) => {
                                this.setState({ testConnectionPending: false,
                                                testConnectionResult: response.result !== undefined ? response.result : TestConnectionResult.ConnectionError });
                             });
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
                                Please check Database name and connection string.
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
                            <Col md={12}>
                                <h3>
                                    <span className="right-space">Connection Settings</span>
                                    { this.state.testConnectionPending ? (
                                        <Icon path={mdiLoading} spin={true} className="icon"/>
                                    ) : this.preRenderConnectionCheckIcon() }
                                </h3>
                            </Col>
                        </Row>
                        <Row>
                            <Col md={12}>
                                <Container fluid={true}>
                                    <Row >
                                        <Col md={12}>
                                           <Input  type="select" placeholder="Configurator Type Name" onChange={(e: React.FormEvent<HTMLInputElement>) => this.onConfiguratorTypeChanged(e)}
                                            value={this.state.config.configuratorTypename} onBlur={() => this.onTestConnection()}>
                                                <option  value={""}>{}</option>
                                             {this.props.DataModel.possibleConfigurators.map((config, idx) =>  (<option key={idx} value={config.configuratorTypename}>{config.name}</option>))}
                                           </Input>
                                        </Col>
                                        {this.state.config.configuratorTypename && this.createEntriesInput()}
                                    </Row>
                                    <Row className="up-space-lg">
                                        <Col md={12}>
                                            <Button color="primary" onClick={() => this.onSave()}>Save</Button>
                                        </Col>
                                    </Row>
                                </Container>
                            </Col>
                            <Col md={12}>
                                <Container fluid={true}>
                                    <Row className="up-space-lg">
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
                                        <Col md={12}>
                                            <h3>Backup &amp; Restore</h3>
                                        </Col>
                                        <Col md={12}>
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
                                        </Col>
                                    </Row>
                                </Container>
                            </Col>
                        </Row>
                        <Row className="up-space-lg">
                            <Col md={12}>
                                <Container fluid={true}>
                                        <Row>
                                            <Col md={12}>
                                                <h3>Database</h3>
                                            </Col>
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
                                                            disabled={this.state.testConnectionResult !== TestConnectionResult.Success && this.state.testConnectionResult !== TestConnectionResult.PendingMigrations}>
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
                                                                    disabled={this.state.selectedMigration === "" || (this.state.testConnectionResult !== TestConnectionResult.Success && this.state.testConnectionResult !== TestConnectionResult.PendingMigrations)}>
                                                                Apply selected migration
                                                            </Button>
                                                            <Button color="primary"
                                                                    onClick={() => this.onRollbackDatabase()}
                                                                    disabled={this.props.DataModel.availableMigrations.length === 0 || (this.state.testConnectionResult !== TestConnectionResult.Success && this.state.testConnectionResult !== TestConnectionResult.PendingMigrations)}>
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
