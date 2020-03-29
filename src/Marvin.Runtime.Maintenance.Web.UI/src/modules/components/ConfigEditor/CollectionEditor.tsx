/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiChevronDown, mdiFolderOpen, mdiPlus, mdiTrashCanOutline } from "@mdi/js";
import Icon from "@mdi/react";
import * as React from "react";
import { Button, ButtonGroup, Col, Collapse, Container, DropdownItem, DropdownMenu, DropdownToggle, Input, Row } from "reactstrap";
import Entry from "../../models/Entry";
import CollapsibleEntryEditorBase, { CollapsibleEntryEditorBasePropModel } from "./CollapsibleEntryEditorBase";
import ConfigEditor from "./ConfigEditor";

interface CollectionEditorStateModel {
    SelectedEntry: string;
    ExpandedEntryNames: string[];
}

export default class CollectionEditor extends CollapsibleEntryEditorBase<CollectionEditorStateModel> {
    constructor(props: CollapsibleEntryEditorBasePropModel) {
        super(props);
        this.state = {
            SelectedEntry: props.Entry.Value.Possible[0],
            ExpandedEntryNames: [],
        };
    }

    public toggleCollapsible(entryName: string): void {
        if (this.isExpanded(entryName)) {
            this.setState((prevState) => ({ ExpandedEntryNames: prevState.ExpandedEntryNames.filter((name) => name !== entryName) }));
        } else {
            this.setState((prevState) => ({ ExpandedEntryNames: [...prevState.ExpandedEntryNames, entryName] }));
        }
    }

    public isExpanded(entryName: string): boolean {
        return this.state.ExpandedEntryNames.find((e: string) => e === entryName) != undefined;
    }

    public onSelect(e: React.FormEvent<HTMLInputElement>): void {
        this.setState({ SelectedEntry: e.currentTarget.value });
    }

    public addEntry(): void {
        const prototype = this.props.Entry.Prototypes.find((proto: Entry) => proto.Key.Name === this.state.SelectedEntry);
        const entryClone = Entry.cloneFromPrototype(prototype, this.props.Entry);

        let counter: number = 0;
        let entryName: string = entryClone.Key.Name;

        while (this.props.Entry.SubEntries.find((subEntry: Entry) => subEntry.Key.Name === entryName) !== undefined) {
            ++counter;
            entryName = entryClone.Key.Name + " " + counter.toString();
        }

        entryClone.Key.Name = entryName;
        this.props.Entry.SubEntries.push(entryClone);

        this.forceUpdate();
    }

    public removeEntry(entry: Entry): void {
        this.props.Entry.SubEntries.splice(this.props.Entry.SubEntries.indexOf(entry), 1);
        this.forceUpdate();
    }

    public preRenderOptions(): React.ReactNode {
        const options: React.ReactNode[] = [];
        this.props.Entry.Value.Possible.map((colEntry, idx) =>
        (
            options.push(<option key={idx} value={colEntry}>{colEntry}</option>)
        ));
        return options;
    }

    public preRenderConfigEditor(entry: Entry): React.ReactNode {
        return <ConfigEditor ParentEntry={entry}
                             Entries={entry.SubEntries}
                             Root={this.props.Root}
                             navigateToEntry={this.props.navigateToEntry}
                             IsReadOnly={this.props.IsReadOnly} />;
    }

    public render(): React.ReactNode {
        return (
            <div className="up-space">
                <Collapse isOpen={this.props.IsExpanded}>
                    <Container fluid={true} className="no-padding up-space down-space">
                        {
                            this.props.Entry.SubEntries.map((entry, idx) =>
                                <div key={idx}>
                                    <Row className="table-row down-space">
                                        <Col md={6} className="no-padding">{entry.Key.Name}</Col>
                                        <Col md={6} className="no-padding">
                                            <ButtonGroup>
                                                <Button color="secondary" onClick={() => this.props.navigateToEntry(entry)}>
                                                    <Icon path={mdiFolderOpen} className="icon right-space" />
                                                    Open
                                                </Button>
                                                <Button color="secondary" onClick={() => this.toggleCollapsible(entry.Key.UniqueIdentifier)}>
                                                    <Icon path={mdiChevronDown} className="icon right-space" />
                                                    Expand
                                                </Button>
                                                <Button color="secondary" onClick={() => this.removeEntry(entry)} disabled={this.props.Entry.Value.IsReadOnly || this.props.IsReadOnly}>
                                                    <Icon path={mdiTrashCanOutline} className="icon right-space" />
                                                    Remove
                                                </Button>
                                            </ButtonGroup>
                                        </Col>
                                    </Row>
                                    <Collapse isOpen={this.isExpanded(entry.Key.UniqueIdentifier)}>
                                        {this.preRenderConfigEditor(entry)}
                                    </Collapse>
                                </div>,
                            )
                        }
                        <Row>
                            <Col md={12} className="no-padding">
                                <Input type="select" value={this.state.SelectedEntry}
                                       className="right-space"
                                       style={{display: "inline", width: "60%"}}
                                       disabled={this.props.Entry.Value.IsReadOnly || this.props.IsReadOnly}
                                       onChange={(e: React.FormEvent<HTMLInputElement>) => this.onSelect(e)}>
                                    {this.preRenderOptions()}
                                </Input>
                                <Button color="primary" onClick={() => this.addEntry()} disabled={this.props.Entry.Value.IsReadOnly || this.props.IsReadOnly}>
                                    <Icon path={mdiPlus} className="icon-white right-space" />
                                    Add entry
                                </Button>
                            </Col>
                        </Row>
                    </Container>
                </Collapse>
            </div>
        );
    }
}
