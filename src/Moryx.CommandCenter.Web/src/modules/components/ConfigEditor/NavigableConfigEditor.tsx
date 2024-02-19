/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Action, History, Location, UnregisterCallback } from "history";
import queryString from "query-string";
import * as React from "react";
import { Button, ButtonGroup, Col, Container, Row } from "reactstrap";
import Entry from "../../models/Entry";
import ConfigEditor from "./ConfigEditor";

interface NavigableConfigEditorPropModel {
    ParentEntry: Entry;
    Entries: Entry[];
    IsReadOnly: boolean;
    Root: Entry;
    History?: History;
    Location?: Location;
}

interface NavigableConfigEditorStateModel {
    EntryChain: Entry[];
    ParentEntry: Entry;
    Entries: Entry[];
}

export default class NavigableConfigEditor extends React.Component<NavigableConfigEditorPropModel, NavigableConfigEditorStateModel> {
    public unregisterListener: UnregisterCallback;

    constructor(props: NavigableConfigEditorPropModel) {
        super(props);
        this.state = {
            EntryChain: [],
            ParentEntry: this.props.ParentEntry,
            Entries: this.props.Entries
        };
    }

    public componentWillReceiveProps(nextProps: NavigableConfigEditorPropModel): void {
        if (this.props.ParentEntry !== nextProps.ParentEntry ||
            this.props.Entries !== nextProps.Entries) {
            this.setState({ ParentEntry: nextProps.ParentEntry, Entries: nextProps.Entries });
            this.resolveEntryChainByPath(this.props.Location, nextProps.Entries);
        }
    }

    public componentDidMount(): void {
        if (this.props.History !== undefined) {
            this.unregisterListener = this.props.History.listen(this.onHistoryChanged.bind(this));
        }
    }

    public componentWillUnmount(): void {
        if (this.unregisterListener !== undefined) {
            this.unregisterListener();
        }
    }

    private resolveEntryChainByPath(location: Location, entries: Entry[]): void {
        if (location !== undefined) {
            const query = queryString.parse(location.search);
            if (query != null && "path" in query) {
                const entryChain: Entry[] = [];
                let currentEntry: Entry = null;

                (query.path as string).split("/").forEach((element: string) => {
                    const searchableEntries: Entry[] = currentEntry != null ? currentEntry.subEntries : entries;
                    const filtered = searchableEntries.filter((entry) => entry.identifier === element);

                    if (filtered.length > 0) {
                        currentEntry = filtered[0];
                        entryChain.push(currentEntry);
                    }
                });

                this.setState({
                    ParentEntry: currentEntry,
                    Entries: currentEntry != null ? currentEntry.subEntries : entries,
                    EntryChain: entryChain
                });
            } else {
                this.setState({ ParentEntry: null, Entries: entries, EntryChain: [] });
            }
        }
    }

    private onHistoryChanged(location: Location, action: Action): void {
        this.resolveEntryChainByPath(location, this.props.Entries);
    }

    public navigateToEntry(entry: Entry): void {
        this.updatePath(Entry.entryChain(entry));
    }

    private updatePath(entryChain: Entry[]): void {
        this.props.History.push("?path=" + entryChain.map((entry) => entry.identifier).join("/"));
    }

    private onClickBreadcrumb(entry: Entry): void {
        if (entry == null) {
            this.updatePath([]);
        } else {
            const idx = this.state.EntryChain.indexOf(entry);
            const updatedEntryChain = this.state.EntryChain.slice(0, idx + 1);
            this.setState({ EntryChain: updatedEntryChain, });

            this.updatePath(updatedEntryChain);
        }
    }

    private preRenderBreadcrumb(): React.ReactNode {
        const entryChainButtons = this.state.EntryChain.map((entry, idx) =>
        (
            <Button key={idx} color="light" onClick={() => this.onClickBreadcrumb(entry)} disabled={idx === this.state.EntryChain.length - 1}>{entry.displayName}</Button>
        ));

        return (
            <ButtonGroup>
                <Button color="dark" disabled={this.state.EntryChain.length === 0} onClick={() => this.onClickBreadcrumb(null)}>Home</Button>
                {entryChainButtons}
            </ButtonGroup>
        );
    }

    public render(): React.ReactNode {
        return (
            <div>
                {this.preRenderBreadcrumb()}
                <Container fluid={true} className="up-space-lg no-padding">
                    <Row className="config-editor-header">
                        <Col md={5} className="no-padding"><span className="font-bold">Property</span></Col>
                        <Col md={7} className="no-padding"><span className="font-bold">Value</span></Col>
                    </Row>
                    <ConfigEditor ParentEntry={this.state.ParentEntry}
                        Entries={this.state.Entries}
                        Root={this.props.Root}
                        navigateToEntry={this.navigateToEntry.bind(this)}
                        IsReadOnly={this.props.IsReadOnly} />
                </Container>
            </div>
        );
    }
}
