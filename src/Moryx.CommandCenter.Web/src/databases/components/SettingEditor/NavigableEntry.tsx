/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Action, History, Location, UnregisterCallback } from "history";
import * as qs from "query-string";
import * as React from "react";
import { Button, ButtonGroup, Col, Container, Row } from "reactstrap";
import Entry from "../../../modules/models/Entry";
import EntryEditor from "./EntryEditor";

interface NavigableEntryPropModel {
    Entry: Entry;
    IsReadOnly: boolean;
}

interface NavigableEntryStateModel {
    Entry: Entry;
}

export default class NavigableEntry extends React.Component<NavigableEntryPropModel, NavigableEntryStateModel> {
    public unregisterListener: UnregisterCallback;

    constructor(props: NavigableEntryPropModel) {
        super(props);
    }

    public componentWillUnmount(): void {
        if (this.unregisterListener !== undefined) {
            this.unregisterListener();
        }
    }

    public render(): React.ReactNode {
        return (
            <div>
                <Container fluid={true} className="up-space-lg no-padding">
                    <EntryEditor Entry={this.props.Entry}
                                  IsReadOnly={this.props.IsReadOnly} />
                </Container>
            </div>
        );
    }
}
