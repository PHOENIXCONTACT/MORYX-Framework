/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiChevronDown, mdiChevronRight, mdiPlus, mdiTrashCanOutline } from "@mdi/js";
import Button from "@mui/material/Button";
import Collapse from "@mui/material/Collapse";
import Container from "@mui/material/Container";
import Grid from "@mui/material/Grid";
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
import CollapsibleEntryEditorBase, { CollapsibleEntryEditorBasePropModel } from "./CollapsibleEntryEditorBase";
import ConfigEditor from "./ConfigEditor";
import EnumEditor from "./EnumEditor";
import NumberEditor from "./NumberEditor";
import StringEditor from "./StringEditor";

interface CollectionEditorStateModel {
    SelectedEntry: string;
    ExpandedEntryNames: string[];
}

export default class CollectionEditor extends CollapsibleEntryEditorBase<CollectionEditorStateModel> {
    constructor(props: CollapsibleEntryEditorBasePropModel) {
        super(props);
        this.state = {
            SelectedEntry: props.Entry.value.possible[0],
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

    public onSelect(e: React.ChangeEvent<HTMLInputElement>): void {
        this.setState({ SelectedEntry: e.target.value });
    }

    public addEntry(): void {
        const prototype = this.props.Entry.prototypes.find((proto: Entry) => proto.displayName === this.state.SelectedEntry);
        const entryPrototype = Entry.entryFromPrototype(prototype, this.props.Entry);
        entryPrototype.identifier = "CREATED";

        let counter: number = 0;
        let entryName: string = entryPrototype.displayName;

        while (this.props.Entry.subEntries.find((subEntry: Entry) => subEntry.displayName === entryName) !== undefined) {
            ++counter;
            entryName = entryPrototype.displayName + " " + counter.toString();
        }

        entryPrototype.displayName = entryName;
        this.props.Entry.subEntries.push(entryPrototype);

        this.forceUpdate();
    }

    public removeEntry(entry: Entry): void {
        this.props.Entry.subEntries.splice(this.props.Entry.subEntries.indexOf(entry), 1);
        this.forceUpdate();
    }

    public preRenderOptions(): React.ReactNode {
        const options: React.ReactNode[] = [];
        this.props.Entry.value.possible.map((colEntry, idx) =>
        (
            options.push(<MenuItem   key={idx} value={colEntry}>{colEntry}</MenuItem>)
        ));
        return options;
    }

    public preRenderConfigEditor(entry: Entry): React.ReactNode {
        if (!Entry.isClassOrCollection(entry)) {
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
            }

            return (<span>Not implemented yet: {entry.value.type}</span>);
        }

        return <ConfigEditor ParentEntry={entry}
                             Entries={entry.subEntries}
                             Root={this.props.Root}
                             navigateToEntry={this.props.navigateToEntry}
                             IsReadOnly={this.props.IsReadOnly} />;
    }

    public render(): React.ReactNode {
        return (
            <Collapse in={this.props.IsExpanded}>
                <Grid container={true} item={true} sx={{paddingLeft: 3, paddingRight: 0}}>
                    {
                        this.props.Entry.subEntries.map((entry, idx) => {
                            if (Entry.isClassOrCollection(entry)) {
                                return (
                                    <Grid container={true} key={idx}>
                                        <Grid item={true} md={11} paddingTop={0.5} paddingRight={0}>
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
                                        </Grid>
                                        <Grid container={true} item={true} md={1} justifyContent="flex-end">
                                            <IconButton
                                                onClick={() => this.removeEntry(entry)}
                                                disabled={this.props.Entry.value.isReadOnly || this.props.IsReadOnly}
                                            >
                                                <SvgIcon><path d={mdiTrashCanOutline} /></SvgIcon>
                                            </IconButton>
                                        </Grid>
                                        <Grid container={true} item={true}  direction="column" alignItems="stretch">
                                            <Collapse in={this.isExpanded(entry.uniqueIdentifier)}>
                                                <Grid item={true} sx={{paddingLeft: 3, paddingRight: 0}} md={12}>
                                                {this.preRenderConfigEditor(entry)}
                                                </Grid>
                                            </Collapse>
                                        </Grid>
                                    </Grid >
                                );
                            } else {
                                return (
                                    <Grid container={true}>
                                        <Grid item={true} md={11}>{this.preRenderConfigEditor(entry)}</Grid>
                                        <Grid container={true} item={true} md={1} justifyContent="flex-end" alignContent="center">
                                            <IconButton
                                                onClick={() => this.removeEntry(entry)}
                                                disabled={this.props.Entry.value.isReadOnly || this.props.IsReadOnly}
                                            >
                                                <SvgIcon><path d={mdiTrashCanOutline} /></SvgIcon>
                                            </IconButton>
                                        </Grid>
                                    </Grid>
                                );
                            }
                        })
                    }
                    <Grid container={true} sx={{paddingTop: 1, paddingBottom: 1}}>
                        <Grid item={true} md={11}>
                            <TextField
                                select={true}
                                label="Type"
                                value={this.state.SelectedEntry}
                                disabled={this.props.Entry.value.isReadOnly || this.props.IsReadOnly}
                                onChange={(e: React.ChangeEvent<HTMLInputElement>) => this.onSelect(e)}
                                size="small"
                                fullWidth={true}
                            >
                                {this.preRenderOptions()}
                            </TextField>
                        </Grid>
                        <Grid container={true} item={true} md={1} justifyContent="flex-end">
                            <IconButton
                                color="primary"
                                onClick={() => this.addEntry()}
                                disabled={this.props.Entry.value.isReadOnly || this.props.IsReadOnly}
                            >
                                <SvgIcon><path d={mdiPlus} /></SvgIcon>
                            </IconButton>
                        </Grid>
                    </Grid>
                </Grid>
            </Collapse>
        );
    }
}
