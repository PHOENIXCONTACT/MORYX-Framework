/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiChevronDown, mdiChevronRight } from "@mdi/js";
import Button from "@mui/material/Button";
import GridLegacy from "@mui/material/GridLegacy";
import IconButton from "@mui/material/IconButton";
import MenuItem from "@mui/material/MenuItem";
import SvgIcon from "@mui/material/SvgIcon";
import TextField from "@mui/material/TextField";
import Tooltip from "@mui/material/Tooltip";
import * as React from "react";
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

    constructor(props: ConfigEditorPropModel) {
        super(props);
        this.state = {
            ExpandedEntryNames: [],
            SelectedEntryType: this.props.ParentEntry != null ? this.props.ParentEntry.value.current : ""
        };
    }

    public componentDidUpdate(prevProps: Readonly<ConfigEditorPropModel>, prevState: Readonly<ConfigEditorStateModel>, snapshot?: any): void {
        if (this.props.ParentEntry && (prevProps.ParentEntry?.value.current !== this.props.ParentEntry.value.current)) {
            this.setState({SelectedEntryType: this.props.ParentEntry.value.current});

        }
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

    private onEntryTypeChange(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({SelectedEntryType: e.target.value});
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
                    <div>
                        <IconButton
                            sx={{padding: 0}}
                            onClick={() => this.toggleCollapsible(entry.uniqueIdentifier ?? entry.identifier)}>
                            <SvgIcon><path d={this.isExpanded(entry.uniqueIdentifier ?? entry.identifier) ? mdiChevronDown : mdiChevronRight } /></SvgIcon>
                        </IconButton>
                        <Tooltip title={entry.description} placement="right">
                            <Button onClick={() => this.props.navigateToEntry(entry)}
                                sx={{textTransform: "none"}}
                                >
                                {entry.displayName}
                            </Button>
                        </Tooltip>
                    </div>
                );
            }
        }

        return (<span>Not implemented yet: {entry.value.type}</span>);
    }

    public preRenderEntries(entries: Entry[]): React.ReactNode {
        return entries.map((subEntry) =>
        (
            <GridLegacy container={true} sx={{ marginTop: 0.5 }} key={subEntry.identifier}>
                <GridLegacy container={true} item={true} md={12} direction="column" alignItems="stretch">
                {
                    this.selectPropertyByType(subEntry)
                }
                </GridLegacy>

                { subEntry.value.type === EntryValueType.Collection &&
                    (
                        <GridLegacy container={true} item={true} md={12} direction="column" alignItems="stretch">
                            <CollectionEditor Entry={subEntry}
                                                IsExpanded={this.isExpanded(subEntry.uniqueIdentifier ?? subEntry.identifier)}
                                                Root={this.props.Root}
                                                navigateToEntry={this.props.navigateToEntry}
                                                IsReadOnly={this.props.IsReadOnly} />
                        </GridLegacy>
                    )
                }
                { subEntry.value.type === EntryValueType.Class &&
                    (
                        <GridLegacy item={true} md={12} direction="column" alignItems="stretch">
                            <ClassEditor Entry={subEntry}
                                            IsExpanded={this.isExpanded(subEntry.uniqueIdentifier ?? subEntry.identifier)}
                                            Root={this.props.Root}
                                            navigateToEntry={this.props.navigateToEntry}
                                            IsReadOnly={this.props.IsReadOnly} />
                        </GridLegacy>
                    )
                }
            </GridLegacy>
        ));
    }

    public render(): React.ReactNode {
        let entries: any;
        if (this.props.ParentEntry != null && this.props.ParentEntry.value.type === EntryValueType.Collection) {
            entries = (
                <GridLegacy container={true}>
                    <GridLegacy container={true} item={true} md={12} direction="column" alignItems="stretch">
                        <CollectionEditor Entry={this.props.ParentEntry}
                                          IsExpanded={true}
                                          Root={this.props.Root}
                                          navigateToEntry={this.props.navigateToEntry}
                                          IsReadOnly={this.props.IsReadOnly} />
                    </GridLegacy>
                </GridLegacy>
            );
        } else {
            entries = this.preRenderEntries(this.props.Entries);
        }

        return (
            <GridLegacy container={true} item={true} md={12}>
                {entries}
                { ConfigEditor.isEntryTypeSettable(this.props.ParentEntry) &&
                    <GridLegacy container={true}>
                        <GridLegacy container={true} item={true} md={12}>
                            <TextField
                                select={true}
                                value={this.state.SelectedEntryType}
                                fullWidth={true}
                                size="small"
                                label="Type"
                                margin="dense"
                                onChange={(e: React.ChangeEvent<HTMLInputElement>) => this.onEntryTypeChange(e)}>
                                {
                                    this.props.ParentEntry.value.possible.map((possibleValue, idx) => {
                                        return (<MenuItem key={idx} value={possibleValue.key}>{possibleValue.displayName}</MenuItem>);
                                    })
                                }
                            </TextField>
                            <Button color="primary"
                                    disabled={this.state.SelectedEntryType === "" || this.state.SelectedEntryType === this.props.ParentEntry.value.current}
                                    onClick={() => this.onPatchToSelectedEntryType()}>
                                Set entry type
                            </Button>
                        </GridLegacy>
                    </GridLegacy>
                }
            </GridLegacy>
        );
    }
}
