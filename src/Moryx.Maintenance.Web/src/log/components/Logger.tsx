/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as moment from "moment";
import * as React from "react";
import NotificationSystem = require("react-notification-system");
import { Button, ButtonGroup, Col, Container, Input, Modal, ModalBody, ModalFooter, ModalHeader, Row, Table } from "reactstrap";
import LogRestClient from "../api/LogRestClient";
import LogLevelToCssClassConverter from "../converter/LogLevelToCssClassConverter";
import LoggerModel from "../models/LoggerModel";
import { LogLevel } from "../models/LogLevel";
import LogMessageModel from "../models/LogMessageModel";

interface LogPropsModel {
    RestClient: LogRestClient;
    Logger: LoggerModel;
    onCloseTab(logger: LoggerModel): void;
}

interface LogStateModel {
    LogMessages: LogMessageModel[];
    FilteredLogMessages: LogMessageModel[];
    AppenderId: number;
    MaxLogEntries: number;
    IntermediateMaxLogEntries: number;
    FilterLogLevel: LogLevel;
    SelectedLogMessage: LogMessageModel;
    IsLogDetailDialogOpen: boolean;
}

export default class Logger extends React.Component<LogPropsModel, LogStateModel> {
    private updateLogMessagesTimer: NodeJS.Timeout;

    constructor(props: LogPropsModel) {
        super(props);
        this.state = {
            LogMessages: [],
            FilteredLogMessages: [],
            AppenderId: -1,
            MaxLogEntries: 20,
            IntermediateMaxLogEntries: 20,
            FilterLogLevel: LogLevel.Info,
            SelectedLogMessage: null,
            IsLogDetailDialogOpen: false,
        };
    }

    public componentDidMount(): void {
        this.props.RestClient.addRemoteAppender(this.props.Logger.Name, this.props.Logger.ActiveLevel).then((data) => {
            this.setState({ AppenderId: data.appenderId });
            this.onUpdateLogMessages();
            this.updateLogMessagesTimer = setInterval(this.onUpdateLogMessages.bind(this), 3000);
        });
    }

    public componentWillUnmount(): void {
        clearInterval(this.updateLogMessagesTimer);
        if (this.state.AppenderId !== undefined) {
            this.props.RestClient.removeRemoteAppender(this.state.AppenderId);
        }
    }

    public render(): React.ReactNode {
        return (
            <div>
                <Container fluid={true}>
                    <Row style={{marginTop: "15px"}}>
                        <Col md={2} className="font-bold font-small">Filter by log level:</Col>
                        <Col md={2}>
                            <Input type="select" value={this.state.FilterLogLevel}
                                onChange={(e: React.FormEvent<HTMLInputElement>) => this.onFilterLogLevelChange(e)}>
                                <option value={LogLevel.Trace}>Trace</option>
                                <option value={LogLevel.Debug}>Debug</option>
                                <option value={LogLevel.Info}>Info</option>
                                <option value={LogLevel.Warning}>Warning</option>
                                <option value={LogLevel.Error}>Error</option>
                                <option value={LogLevel.Fatal}>Fatal</option>
                            </Input>
                        </Col>
                        <Col md={2} className="font-bold font-small">Max. log entries:</Col>
                        <Col md={2}>
                            <Input type="text" value={this.state.IntermediateMaxLogEntries}
                                   onChange={(e: React.FormEvent<HTMLInputElement>) => this.onChangeMaxEntries(e)}
                                   onBlur={this.onApplyMaxEntries.bind(this)} />
                        </Col>
                        <Col md={1} />
                        <Col md={3}>
                            <ButtonGroup className="float-right">
                                <Button color="primary" onClick={this.clearLogMessages.bind(this)} disabled={this.state.LogMessages.length === 0}>Clear log messages</Button>
                                { this.props.onCloseTab != null &&
                                    <Button color="primary" onClick={() => this.props.onCloseTab(this.props.Logger)}>Close tab</Button>
                                }
                            </ButtonGroup>
                        </Col>
                    </Row>
                    <Row style={{marginTop: "10px"}}>
                        <Col md={12}>
                            <Table size="sm">
                                <thead>
                                    <tr>
                                        <th>Timestamp</th>
                                        <th>Level</th>
                                        <th>Message</th>
                                        <th>Class name</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    { this.state.LogMessages.length !== 0 && (
                                        this.preRenderLogMessages()
                                    )}
                                </tbody>
                                { this.state.LogMessages.length === 0 && (
                                    <tbody>
                                        <tr>
                                            <td>
                                                <span className="font-normal font-italic">No log messages found for this logger.</span>
                                            </td>
                                        </tr>
                                    </tbody>
                                )}
                            </Table>
                        </Col>
                    </Row>
                </Container>
                { this.state.SelectedLogMessage != null &&
                    <Modal isOpen={this.state.IsLogDetailDialogOpen} className="log-modal-dialog">
                        <ModalHeader tag="h2" style={{background: LogLevelToCssClassConverter.Convert(this.state.SelectedLogMessage.LogLevel)}}>
                            Log message from {moment(this.state.SelectedLogMessage.Timestamp).format("YYYY-MM-DD HH:mm:ss")} ({this.state.SelectedLogMessage.ClassName})
                        </ModalHeader>
                        <ModalBody>
                            <Container fluid={true}>
                                <Row>
                                    <Col md={12}>
                                        <pre className="font-small">{this.state.SelectedLogMessage.Message}</pre>
                                    </Col>
                                </Row>
                            </Container>
                        </ModalBody>
                        <ModalFooter>
                            <Button color="primary" onClick={() => this.setState({ SelectedLogMessage: null, IsLogDetailDialogOpen: false })}>Close</Button>
                        </ModalFooter>
                    </Modal>
                }
            </div>
        );
    }

    private onUpdateLogMessages(): void {
        this.props.RestClient.messages(this.state.AppenderId).then((data) => this.updateLogMessages(data));
    }

    private updateLogMessages(logMessages: LogMessageModel[]): void {
        logMessages.reverse();
        const newLogMessageList = logMessages.concat(this.state.LogMessages);
        this.setState({ LogMessages: newLogMessageList, FilteredLogMessages: Logger.applyFilter(newLogMessageList, this.state.FilterLogLevel, this.state.MaxLogEntries) });
    }

    private clearLogMessages(): void {
        this.setState({ LogMessages: [], FilteredLogMessages: [] });
    }

    private onFilterLogLevelChange(e: React.FormEvent<HTMLInputElement>): void {
        const newValue = (e.target as HTMLSelectElement).value  as LogLevel;
        this.setState({ FilterLogLevel: newValue, FilteredLogMessages: Logger.applyFilter(this.state.LogMessages, newValue, this.state.MaxLogEntries) });
    }

    private onChangeMaxEntries(e: React.FormEvent<HTMLInputElement>): void {
        this.setState({ IntermediateMaxLogEntries: parseInt(e.currentTarget.value, 10) });
    }

    private onApplyMaxEntries(): void {
        this.setState({ MaxLogEntries: this.state.IntermediateMaxLogEntries, FilteredLogMessages: Logger.applyFilter(this.state.LogMessages, this.state.FilterLogLevel, this.state.IntermediateMaxLogEntries) });
    }

    private static applyFilter(logMessages: LogMessageModel[], logLevel: LogLevel, maxEntries: number): LogMessageModel[] {
        return logMessages.filter((logMessage: LogMessageModel) => logMessage.LogLevel >= logLevel).slice(0, maxEntries);
    }

    private onShowLogMessageDetailed(logMessage: LogMessageModel): void {
        this.setState({ SelectedLogMessage: logMessage, IsLogDetailDialogOpen: true });
    }

    private static cutMessage(message: string): string {
        const lines: string[] = message.split("\n");
        let cutted = lines.length > 0 ? lines[0] : message;
        cutted = cutted.slice(0, 150);

        if (cutted.length < message.length) {
            cutted += "...";
        }

        return cutted;
    }

    private preRenderLogMessages(): React.ReactNode {
        return this.state.FilteredLogMessages.map((message, idx) =>
            <tr key={idx}
                className={"selectable"}
                style={{background: LogLevelToCssClassConverter.Convert(message.LogLevel)}}
                onClick={this.onShowLogMessageDetailed.bind(this, message)}>
                <td>{moment(message.Timestamp).format("YYYY-MM-DD HH:mm:ss")}</td>
                <td>{LogLevel[message.LogLevel]}</td>
                <td>{Logger.cutMessage(message.Message)}</td>
                <td>{message.ClassName}</td>
            </tr>,
        );
    }
}
