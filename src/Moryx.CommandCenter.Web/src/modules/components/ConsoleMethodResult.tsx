/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { Button, Col, Container, Row } from "reactstrap";
import Entry from "../models/Entry";
import MethodEntry from "../models/MethodEntry";
import NavigableConfigEditor from "./ConfigEditor/NavigableConfigEditor";

export interface ConsoleMethodResultPropModel {
    Method: MethodEntry;
    InvokeResult: Entry;
    onResetInvokeResult(methodName: string): void;
}

function ConsoleMethodResult(props: ConsoleMethodResultPropModel) {
    const resetInvokeResult = (): void => {
        props.onResetInvokeResult(props.Method.name);
    };

    return (
        <div>
            {props.InvokeResult == null ? (
                <span className="font-italic">There is no result.</span>
            ) : (
                <Container fluid={true} className="no-padding">
                    <Row>
                        <Col md={3}><span className="font-bold">Name:</span></Col>
                        <Col md={9}><span className="font-italic">{props.Method.displayName}</span></Col>
                    </Row>
                    <Row>
                        <Col md={3}><span className="font-bold">Description:</span></Col>
                        <Col md={9}><span className="font-italic">{props.Method.description}</span></Col>
                    </Row>
                    <Row>
                        <Col md={12} className="up-space-lg">
                            <NavigableConfigEditor
                                Entries={props.InvokeResult.subEntries}
                                ParentEntry={null}
                                Root={props.InvokeResult}
                                IsReadOnly={true} />
                        </Col>
                    </Row>
                    <Row className="up-space-lg">
                        <Col md={12}>
                            <Button color="primary" className="float-right" onClick={resetInvokeResult}>
                                Reset result
                            </Button>
                        </Col>
                    </Row>
                </Container>
            )}
        </div>
    );
}

export default ConsoleMethodResult;
