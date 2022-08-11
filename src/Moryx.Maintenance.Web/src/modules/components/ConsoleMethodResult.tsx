/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { RouteComponentProps, withRouter } from "react-router-dom";
import { Button, Col, Container, Row } from "reactstrap";
import Entry from "../models/Entry";
import MethodEntry from "../models/MethodEntry";
import NavigableConfigEditor from "./ConfigEditor/NavigableConfigEditor";

export interface ConsoleMethodResultPropModel {
    Method: MethodEntry;
    InvokeResult: Entry;
    onResetInvokeResult(methodName: string): void;
}

class ConsoleMethodResult extends React.Component<ConsoleMethodResultPropModel & RouteComponentProps<{}>> {
    constructor(props: ConsoleMethodResultPropModel & RouteComponentProps<{}>) {
        super(props);
        this.state = { };
    }

    private resetInvokeResult(): void {
        this.props.onResetInvokeResult(this.props.Method.name);
    }

    public render(): React.ReactNode {
        return (
            <div>
                { this.props.InvokeResult == null ? (
                    <span className="font-italic">There is no result.</span>
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
                                <NavigableConfigEditor Entries={this.props.InvokeResult.subEntries}
                                                       ParentEntry={null}
                                                       Root={this.props.InvokeResult}
                                                       IsReadOnly={true}
                                                       History={this.props.history}
                                                       Location={this.props.location} />
                            </Col>
                        </Row>
                        <Row className="up-space-lg">
                            <Col md={12}>
                                <Button color="primary"
                                        className="float-right"
                                        onClick={() => this.resetInvokeResult()}>
                                    Reset result
                                </Button>
                            </Col>
                        </Row>
                    </Container>
                )}
            </div>
        );
    }
}

export default withRouter<ConsoleMethodResultPropModel & RouteComponentProps<{}>, React.ComponentType<any>>(ConsoleMethodResult);
