/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiCheck, mdiHexagon, mdiPlay, mdiRestart, mdiStop} from "@mdi/js";
import Icon from "@mdi/react";
import * as React from "react";
import { connect } from "react-redux";
import { Link } from "react-router-dom";
import { Button, ButtonGroup, Card, CardBody, CardHeader, Col, Container, Input, Modal, ModalBody, ModalFooter, ModalHeader, Row, Table } from "reactstrap";
import { ActionType } from "../../common/redux/Types";
import { HealthStateBadge } from "../../dashboard/components/HealthStateBadge";
import ModulesRestClient from "../api/ModulesRestClient";
import { ModuleNotificationTypeToCssClassConverter } from "../converter/ModuleNotificationTypeToCssClassConverter";
import { FailureBehaviour } from "../models/FailureBehaviour";
import { ModuleStartBehaviour } from "../models/ModuleStartBehaviour";
import NotificationModel from "../models/NotificationModel";
import SerializableException from "../models/SerializableException";
import ServerModuleModel from "../models/ServerModuleModel";
import { Serverity } from "../models/Severity";
import { updateFailureBehaviour, updateStartBehaviour } from "../redux/ModulesActions";

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
        const warningsOrErrors = nextProps.Module.Notifications.filter(function(element: NotificationModel, index: number, array: NotificationModel[]): boolean {
             return element.Severity === Serverity.Warning || element.Severity === Serverity.Error || element.Severity === Serverity.Fatal;
        });
        this.setState({ HasWarningsOrErrors: warningsOrErrors.length !== 0 });
    }

    public startModule(): void {
        this.props.RestClient.startModule(this.props.Module.Name);
    }

    public stopModule(): void {
        this.props.RestClient.stopModule(this.props.Module.Name);
    }

    public reincarnateModule(): void {
        this.props.RestClient.reincarnateModule(this.props.Module.Name);
    }

    public confirmModuleWarning(): void {
        this.props.RestClient.confirmModuleWarning(this.props.Module.Name);
    }

    public onStartBehaviourChange(e: React.FormEvent<HTMLInputElement>): void {
        const newValue = parseInt((e.target as HTMLSelectElement).value, 10);
        this.props.RestClient.updateModule({ ...this.props.Module, StartBehaviour: newValue }).then((d) => this.props.onUpdateStartBehaviour(this.props.Module.Name, newValue));
    }

    public onFailureBehaviourChange(e: React.FormEvent<HTMLInputElement>): void {
        const newValue = parseInt((e.target as HTMLSelectElement).value, 10);
        this.props.RestClient.updateModule({ ...this.props.Module, FailureBehaviour: newValue }).then((d) => this.props.onUpdateFailureBehaviour(this.props.Module.Name, newValue));
    }

    private openNotificationDetailsDialog(e: React.MouseEvent<HTMLElement>, notification: NotificationModel): void {
        if (notification.Exception != null) {
            this.setState({ IsNotificationDialogOpened: true, SelectedNotification: notification });
        }
    }

    private closeNotificationDetailsDialog(): void {
        this.setState({ IsNotificationDialogOpened: false, SelectedNotification: null });
    }

    private static preRenderInnerException(exception: SerializableException): React.ReactNode {
        return (
            <div style={{margin: "0px 0px 0px 5px"}}>
                <Container fluid={true}>
                    <Row>
                        <Col md={2}><span className="font-bold">Type</span></Col>
                        <Col md={10}>
                            <span>{exception.ExceptionTypeName}</span>
                        </Col>
                    </Row>
                    <Row>
                        <Col md={2}><span className="font-bold">Message</span></Col>
                        <Col md={10}><span className="font-italic">{exception.Message}</span></Col>
                    </Row>
                </Container>
                {exception.InnerException != null &&
                    Module.preRenderInnerException(exception.InnerException)
                }
            </div>
        );
    }

    public render(): React.ReactNode {
        return (
            <Card>
                <CardHeader tag="h2">
                    <Icon path={mdiHexagon} className="icon right-space" />
                    {this.props.Module.Name} - General
                </CardHeader>
                <CardBody>
                    <Container fluid={true}>
                        <Row>
                            <Col md={6}>
                                <h3>Control</h3>
                                <ButtonGroup>
                                    <Button color="primary" onClick={this.startModule.bind(this)}><Icon path={mdiPlay} className="icon-white right-space" />Start</Button>
                                    <Button color="primary" onClick={this.stopModule.bind(this)}><Icon path={mdiStop} className="icon-white right-space" />Stop</Button>
                                    <Button color="primary" onClick={this.reincarnateModule.bind(this)}><Icon path={mdiRestart} className="icon-white right-space" />Reincarnate</Button>
                                </ButtonGroup>
                            </Col>
                            <Col md={6}>
                                <h3>Error Handling</h3>
                                {this.state.HasWarningsOrErrors ? (
                                    <Button color="warning" onClick={this.confirmModuleWarning.bind(this)}><Icon path={mdiCheck} className="icon right-space" />Confirm</Button>
                                ) : (
                                    <span className="font-italic font-small">No warnings or errors.</span>
                                )}
                            </Col>
                        </Row>
                        <Row className="up-space-lg">
                            <Col md={6}>
                                <h3>General Information</h3>
                                <Container fluid={true}>
                                    <Row>
                                        <Col md={4}><span className="font-bold font-small">Name:</span></Col>
                                        <Col md={8}><span className="font-small font-italic">{this.props.Module.Name}</span></Col>
                                    </Row>
                                    <Row>
                                        <Col md={4}><span className="font-bold font-small">Bundle:</span></Col>
                                        <Col md={8}><span className="font-small font-italic">{this.props.Module.Assembly.Bundle}</span></Col>
                                    </Row>
                                    <Row>
                                        <Col md={4}><span className="font-bold font-small">State:</span></Col>
                                        <Col md={8}><span className="font-small"><HealthStateBadge HealthState={this.props.Module.HealthState} /></span></Col>
                                    </Row>
                                    <Row>
                                        <Col md={4}><span className="font-bold font-small">Assembly:</span></Col>
                                        <Col md={8}><span className="font-small font-italic">{this.props.Module.Assembly.Name}</span></Col>
                                    </Row>
                                </Container>
                            </Col>
                            <Col md={6}>
                                <h3>Dependencies</h3>
                                {this.props.Module.Dependencies.length === 0 ? (
                                    <span className="font-italic font-small">This module has no dependencies.</span>
                                ) : (
                                    <Table striped={true}>
                                        <thead>
                                            <tr>
                                                <th>Module Name</th>
                                                <th>State</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {
                                                this.props.Module.Dependencies.map((module, idx) =>
                                                <tr key={idx}>
                                                    <td><Link to={"/modules/" + module.Name}>{module.Name}</Link></td>
                                                    <td><HealthStateBadge HealthState={module.HealthState} /></td>
                                                </tr>)
                                            }
                                        </tbody>
                                    </Table>
                                )}
                            </Col>
                        </Row>
                        <Row className="up-space-lg">
                            <Col md={6}>
                                <h3>Start &amp; Failure behaviour</h3>
                                <Container fluid={true}>
                                    <Row>
                                        <Col md={4}><span className="font-bold font-small center-text">Start behaviour:</span></Col>
                                        <Col md={8}>
                                            <Input type="select" value={this.props.Module.StartBehaviour}
                                                   onChange={(e: React.FormEvent<HTMLInputElement>) => this.onStartBehaviourChange(e)}>
                                                <option value={ModuleStartBehaviour.Auto}>Auto</option>
                                                <option value={ModuleStartBehaviour.Manual}>Manual</option>
                                                <option value={ModuleStartBehaviour.OnDependency}>On dependency</option>
                                            </Input>
                                        </Col>
                                    </Row>
                                    <Row className="up-space">
                                        <Col md={4}><span className="font-bold font-small center-text">Failure behaviour:</span></Col>
                                        <Col md={8}>
                                            <Input type="select" value={this.props.Module.FailureBehaviour}
                                                   onChange={(e: React.FormEvent<HTMLInputElement>) => this.onFailureBehaviourChange(e)}>
                                                <option value={FailureBehaviour.Stop}>Stop</option>
                                                <option value={FailureBehaviour.StopAndNotify}>Stop and notify</option>
                                                <option value={FailureBehaviour.Reincarnate}>Reincarnate</option>
                                                <option value={FailureBehaviour.ReincarnateAndNotify}>Reincarnate and notify</option>
                                            </Input>
                                        </Col>
                                    </Row>
                                </Container>
                            </Col>
                        </Row>
                        <Row className="up-space-lg">
                            <Col md={12}>
                                <h3>Notifications</h3>
                                {this.props.Module.Notifications.length === 0 ? (
                                    <span className="font-italic font-small">No notifications detected.</span>
                                ) : (
                                    <Table>
                                        <thead>
                                            <tr>
                                                <th>Type</th>
                                                <th>Message</th>
                                                <th>Level</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        {
                                            this.props.Module.Notifications.map((notification, idx) =>
                                                <tr key={idx} className={"selectable"} style={{alignItems: "center"}}
                                                    onClick={(e: React.MouseEvent<HTMLElement>) => this.openNotificationDetailsDialog(e, notification)}>
                                                    <td><span className="align-self-center">{notification.Exception != null ? notification.Exception.ExceptionTypeName : "-"}</span></td>
                                                    <td><span className="align-self-center">{notification.Exception != null ? notification.Exception.Message : notification.Message}</span></td>
                                                    <td>
                                                        <span className="align-self-center" style={ModuleNotificationTypeToCssClassConverter.Convert(notification.Severity)}>
                                                            {Serverity[notification.Severity]}
                                                        </span>
                                                    </td>
                                                </tr>,
                                            )
                                        }
                                        </tbody>
                                    </Table>
                                )}
                            </Col>
                        </Row>
                    </Container>
                </CardBody>
                <Modal isOpen={this.state.IsNotificationDialogOpened} className="notification-modal-dialog">
                    <ModalHeader tag="h2">Notification details</ModalHeader>
                    <ModalBody>
                        {this.state.SelectedNotification != null &&
                            <Container fluid={true}>
                                <Row>
                                    <Col md={2}><span className="font-bold">Type</span></Col>
                                    <Col md={10}>
                                        <span style={ModuleNotificationTypeToCssClassConverter.Convert(this.state.SelectedNotification.Severity)}>
                                            {this.state.SelectedNotification.Exception.ExceptionTypeName}
                                        </span>
                                    </Col>
                                </Row>
                                <Row>
                                    <Col md={2}><span className="font-bold">Message</span></Col>
                                    <Col md={10}><span className="font-italic">{this.state.SelectedNotification.Exception.Message}</span></Col>
                                </Row>
                                <Row>
                                    <Col md={2}><span className="font-bold">Stack trace</span></Col>
                                    <Col md={10}>{this.state.SelectedNotification.Exception.StackTrace}</Col>
                                </Row>
                                <Row>
                                    <Col md={12}>
                                        { this.state.SelectedNotification.Exception.InnerException == null ? (
                                            <span className="font-italic">No inner exception found.</span>
                                        ) : (
                                            <span className="font-bold">Inner exception</span>
                                        )}
                                    </Col>
                                </Row>
                                { this.state.SelectedNotification.Exception.InnerException != null &&
                                    <Row>
                                        <Col md={12}>{Module.preRenderInnerException(this.state.SelectedNotification.Exception.InnerException)}</Col>
                                    </Row>
                                }
                            </Container>
                        }
                    </ModalBody>
                    <ModalFooter>
                        <Button color="primary" onClick={this.closeNotificationDetailsDialog.bind(this)}>Close</Button>
                    </ModalFooter>
                </Modal>
            </Card>
        );
    }
}

export default connect<{}, ModuleDispatchPropModel>(null, mapDispatchToProps)(Module);
