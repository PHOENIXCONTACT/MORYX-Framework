/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiChevronDown, mdiChevronUp, mdiFolderOpen} from "@mdi/js";
import Icon from "@mdi/react";
import * as React from "react";
import { Button, ButtonGroup, Card, CardBody, CardHeader, Col, Container, Dropdown, DropdownItem, DropdownMenu, DropdownToggle, Input, Row, Table } from "reactstrap";
import Entry from "../../models/Entry";
import { EntryValueType } from "../../models/EntryValueType";
import BooleanEditor from "./BooleanEditor";
import ByteEditor from "./ByteEditor";
import ClassEditor from "./ClassEditor";
import CollectionEditor from "./CollectionEditor";
import EnumEditor from "./EnumEditor";
import NumberEditor from "./NumberEditor";
import StringEditor from "./StringEditor";

interface ConfigEditorPropModel {
    ParentEntry: Entry;
    Entries: Entry[];
    IsReadOnly: boolean;
    Root: Entry;
    navigateToEntry(entry: Entry): void;
}

interface ConfigEditorStateModel {
    ExpandedEntryNames: string[];
    SelectedEntryType: string;
}

export default class ConfigEditor extends React.Component<ConfigEditorPropModel, ConfigEditorStateModel> {
    private static divider: number = 2;

    constructor(props: ConfigEditorPropModel) {
        super(props);
        this.state = {
            ExpandedEntryNames: [],
            SelectedEntryType: this.props.ParentEntry != null ? this.props.ParentEntry.value.current : ""
        };
    }

    public componentWillReceiveProps(nextProps: ConfigEditorPropModel): void {
        if (this.state.SelectedEntryType === "" && this.props.ParentEntry != null) {
            this.setState({ SelectedEntryType: this.props.ParentEntry.value.current});
        }
    }

    private toggleCollapsible(entryName: string): void {
        if (this.isExpanded(entryName)) {
            this.setState((prevState) => ({ ExpandedEntryNames: prevState.ExpandedEntryNames.filter((name) => name !== entryName) }));
        } else {
            this.setState((prevState) => ({ ExpandedEntryNames: [...prevState.ExpandedEntryNames, entryName] }));
        }
    }

    private isExpanded(entryName: string): boolean {
        return this.state.ExpandedEntryNames.find((e: string) => e === entryName) != undefined;
    }

    private static isEntryTypeSettable(entry: Entry): boolean {
        if (entry === null || entry === undefined) {
            return false;
        }

        let isEntrySettable = entry.value.type === EntryValueType.Class &&
                              entry.value.possible != null &&
                              entry.value.possible.length > 1;

        if (isEntrySettable) {

            if (entry.parent !== null && entry.parent !== undefined) {
                isEntrySettable = entry.parent.value.type !== EntryValueType.Collection;
            }
        }
        return isEntrySettable;
    }

    private onEntryTypeChange(e: React.FormEvent<HTMLInputElement>): void {
        this.setState({SelectedEntryType: e.currentTarget.value});
    }

    private onPatchToSelectedEntryType(): void {
        let prototype: Entry = null;
        let entryType: EntryValueType = EntryValueType.Class;
        if (this.props.ParentEntry != null) {
            entryType = this.props.ParentEntry.value.type;
        }

        switch (entryType) {
            case EntryValueType.Class:
                prototype = this.props.ParentEntry.prototypes.find((proto: Entry) => proto.displayName === this.state.SelectedEntryType);
                break;
            default:
                return;
        }

        const entryPrototype = Entry.entryFromPrototype(prototype, this.props.ParentEntry.parent);
        entryPrototype.prototypes = JSON.parse(JSON.stringify(this.props.ParentEntry.prototypes));
        entryPrototype.displayName = this.props.ParentEntry.displayName;
        entryPrototype.identifier = this.props.ParentEntry.identifier;

        const subEntries: Entry[] = this.props.ParentEntry.parent.subEntries;

        const idx = subEntries.indexOf(this.props.ParentEntry);
        if (idx !== -1) {
            subEntries[idx] = entryPrototype;
            this.setState({SelectedEntryType: entryPrototype.value.current});
            this.props.navigateToEntry(this.props.ParentEntry);
        }
    }

    public selectPropertyByType(entry: Entry): React.ReactNode {
        switch (entry.value.type) {
            case EntryValueType.Byte: {
                return (<ByteEditor Entry={entry} IsReadOnly={this.props.IsReadOnly} />);
            }
            case EntryValueType.Boolean: {
                return (<BooleanEditor Entry={entry} IsReadOnly={this.props.IsReadOnly} />);
            }
            case EntryValueType.Int16:
            case EntryValueType.UInt16:
            case EntryValueType.Int32:
            case EntryValueType.UInt32:
            case EntryValueType.Int64:
            case EntryValueType.UInt64:
            case EntryValueType.Single:
            case EntryValueType.Double: {
                return (<NumberEditor Entry={entry} IsReadOnly={this.props.IsReadOnly} />);
            }
            case EntryValueType.String: {
                return (<StringEditor Entry={entry} IsReadOnly={this.props.IsReadOnly} />);
            }
            case EntryValueType.Enum: {
                return (<EnumEditor Entry={entry} IsReadOnly={this.props.IsReadOnly} />);
            }
            case EntryValueType.Collection:
            case EntryValueType.Class: {
                return (
                    <ButtonGroup>
                        <Button color="secondary" onClick={() => this.props.navigateToEntry(entry)}>
                            <Icon path={mdiFolderOpen} className="icon right-space" />
                            Open
                        </Button>
                        <Button color="secondary" onClick={() => this.toggleCollapsible(entry.uniqueIdentifier ?? entry.identifier)}>
                            <Icon path={this.isExpanded(entry.uniqueIdentifier ?? entry.identifier) ? mdiChevronUp : mdiChevronDown} className="icon right-space" />
                            {this.isExpanded(entry.uniqueIdentifier ?? entry.identifier) ? "Collapse" : "Expand"}
                        </Button>
                    </ButtonGroup>
                );
            }
        }

        return (<span>Not implemented yet: {entry.value.type}</span>);
    }

    public preRenderEntries(entries: Entry[]): React.ReactNode {
        return entries.map((subEntry, idx) =>
        (
            <div key={idx} className="table-row">
                <Row className="entry-row">
                    <Col md={5} className="no-padding">
                        <Container fluid={true} className="no-padding">
                            <Row>
                                <Col md={12} className="no-padding"><span className="font-bold align-self-center no-padding">{subEntry.displayName}</span></Col>
                            </Row>
                            <Row>
                                <Col md={12} className="no-padding"><span className="font-disabled no-padding">{subEntry.description}</span></Col>
                            </Row>
                        </Container>
                    </Col>
                    <Col md={7} className="no-padding">
                    {
                        this.selectPropertyByType(subEntry)
                    }
                    </Col>
                </Row>
                { subEntry.value.type === EntryValueType.Collection &&
                    (
                        <Row>
                            <Col md={12}>
                                <CollectionEditor Entry={subEntry}
                                                  IsExpanded={this.isExpanded(subEntry.uniqueIdentifier ?? subEntry.identifier)}
                                                  Root={this.props.Root}
                                                  navigateToEntry={this.props.navigateToEntry}
                                                  IsReadOnly={this.props.IsReadOnly} />
                            </Col>
                        </Row>
                    )
                }
                { subEntry.value.type === EntryValueType.Class &&
                    (
                        <Row>
                            <Col md={12}>
                                <ClassEditor Entry={subEntry}
                                             IsExpanded={this.isExpanded(subEntry.uniqueIdentifier ?? subEntry.identifier)}
                                             Root={this.props.Root}
                                             navigateToEntry={this.props.navigateToEntry}
                                             IsReadOnly={this.props.IsReadOnly} />
                            </Col>
                        </Row>
                    )
                }
            </div>
        ));
    }

    public render(): React.ReactNode {
        let entries: any;
        if (this.props.ParentEntry != null && this.props.ParentEntry.value.type === EntryValueType.Collection) {
            entries = (
                <Row>
                    <Col md={12} className="no-padding">
                        <CollectionEditor Entry={this.props.ParentEntry}
                                          IsExpanded={true}
                                          Root={this.props.Root}
                                          navigateToEntry={this.props.navigateToEntry}
                                          IsReadOnly={this.props.IsReadOnly} />
                    </Col>
                </Row>
            );
        } else {
            entries = this.preRenderEntries(this.props.Entries);
        }

        return (
            <div className="config-editor" style={{marginLeft: 10}}>
                {entries}
                { ConfigEditor.isEntryTypeSettable(this.props.ParentEntry) &&
                    <Container fluid={true}
                               style={{margin: "10px 0px 10px 0px"}}
                               className="no-padding">
                        <Row style={{alignItems: "center"}}>
                            <Col md={4} className="no-padding">
                                <Input type="select" value={this.state.SelectedEntryType}
                                       onChange={(e: React.FormEvent<HTMLInputElement>) => this.onEntryTypeChange(e)}>
                                    {
                                        this.props.ParentEntry.value.possible.map((possibleValue, idx) => {
                                            return (<option key={idx}>{possibleValue}</option>);
                                        })
                                    }
                                </Input>
                            </Col>
                            <Col>
                                <Button color="primary"
                                        disabled={this.state.SelectedEntryType === "" || this.state.SelectedEntryType === this.props.ParentEntry.value.current}

                                        onClick={() => this.onPatchToSelectedEntryType()}>
                                    Set entry type
                                </Button>
                            </Col>
                        </Row>
                    </Container>
                }
            </div>
        );
    }
}
