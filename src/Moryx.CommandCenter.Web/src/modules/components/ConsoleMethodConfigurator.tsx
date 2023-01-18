/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { RouteComponentProps, withRouter } from "react-router-dom";
import { Button, Col, Container, Row } from "reactstrap";
import MethodEntry from "../models/MethodEntry";
import NavigableConfigEditor from "./ConfigEditor/NavigableConfigEditor";

export interface ConsoleMethodConfiguratorPropModel {
    ModuleName: string;
    Method: MethodEntry;
    onInvokeMethod(methodEntry: MethodEntry): void;
}

class ConsoleMethodConfigurator extends React.Component<ConsoleMethodConfiguratorPropModel & RouteComponentProps<{}>> {
    constructor(props: ConsoleMethodConfiguratorPropModel & RouteComponentProps<{}>) {
        super(props);
        this.state = { };
    }

    private invokeSelectedMethod(): void {
        this.props.onInvokeMethod(this.props.Method);
    }

    public render(): React.ReactNode {
        return (
            <div>
                { this.props.Method == null ? (
                    <span className="font-italic">Please select a method.</span>
                ) : (
                    <Container fluid={true}
                               className="no-padding">
                        <Row>
                            <Col md={3}><span className="font-bold">Name:</span></Col>
                            <Col md={9}><span className="font-italic">{this.props.Method.displayName}</span></Col>
                        </Row>
                        <Row>
                            <Col md={3}><span className="font-bold">Description:</span></Col>
                            <Col md={9}><span className="font-italic">{this.props.Method.description}</span></Col>
                        </Row>
                        <Row>
                            <Col md={12} className="up-space-lg">
                                { this.props.Method.parameters.subEntries.length === 0 ? (
                                    <span className="font-italic">This method is parameterless.</span>
                                ) : (
                                    <NavigableConfigEditor Entries={this.props.Method.parameters.subEntries}
                                                           ParentEntry={null}
                                                           Root={this.props.Method.parameters}
                                                           IsReadOnly={false}
                                                           History={this.props.history}
                                                           Location={this.props.location} />
                                )}
                            </Col>
                        </Row>
                        <Row className="up-space-lg">
                            <Col md={12}>
                                <Button color="primary"
                                        className="float-right"
                                        onClick={() => this.invokeSelectedMethod()}>
                                    Invoke
                                </Button>
                            </Col>
                        </Row>
                    </Container>
                )}
            </div>
        );
    }
}

export default withRouter<ConsoleMethodConfiguratorPropModel & RouteComponentProps<{}>, React.ComponentType<any>>(ConsoleMethodConfigurator);
