/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Breadcrumbs from "@mui/material/Breadcrumbs";
import Grid from "@mui/material/Grid";
import Link from "@mui/material/Link";
import Typography from "@mui/material/Typography";
import queryString from "query-string";
import * as React from "react";
import { Location, useLocation, useNavigate } from "react-router-dom";
import Entry from "../../models/Entry";
import ConfigEditor from "./ConfigEditor";

interface NavigableConfigEditorPropModel {
    ParentEntry: Entry;
    Entries: Entry[];
    IsReadOnly: boolean;
    Root: Entry;
}

function NavigableConfigEditor(props: NavigableConfigEditorPropModel) {
    const location = useLocation();
    const navigate = useNavigate();
    const [entryChain, setEntryChain] = React.useState<Entry[]>([]);
    const [parentEntry, setParentEntry] = React.useState<Entry | null>(props.ParentEntry);
    const [entries, setEntries] = React.useState<Entry[]>(props.Entries);

    React.useEffect(() => {
        resolveEntryChainByPath(location, props.Entries);
    }, [location, props.Entries]);

    const resolveEntryChainByPath = (location: Location, entries: Entry[]): void => {
        if (location !== undefined) {
            const query = queryString.parse(location.search);
            if (query != null && "path" in query) {
                const entryChain: Entry[] = [];
                let currentEntry: Entry | null = null;

                (query.path as string).split("/").forEach((element: string) => {
                    const searchableEntries: Entry[] = currentEntry != null ? currentEntry.subEntries : entries;
                    const filtered = searchableEntries.filter((entry) => entry.identifier === element);

                    if (filtered.length > 0) {
                        currentEntry = filtered[0];
                        entryChain.push(currentEntry);
                    }
                });

                setParentEntry(currentEntry);
                setEntries(currentEntry != null ? currentEntry.subEntries : entries);
                setEntryChain(entryChain);
            } else {
                setParentEntry(null);
                setEntries(entries);
                setEntryChain([]);
            }
        }
    };

    const navigateToEntry = (entry: Entry): void => {
        updatePath(Entry.entryChain(entry));
    };

    const updatePath = (entryChain: Entry[]): void => {
        navigate("?path=" + entryChain.map((entry) => entry.identifier).join("/"));
    };

    const onClickBreadcrumb = (entry: Entry | null): void => {
        if (entry === null) {
            updatePath([]);
        } else {
            const idx = entryChain.indexOf(entry);
            const updatedEntryChain = entryChain.slice(0, idx + 1);
            setEntryChain(updatedEntryChain);
            updatePath(updatedEntryChain);
        }
    };

    const preRenderBreadcrumb = (): React.ReactNode => {
        const entryChainButtons = entryChain.map((entry, idx) => {
            if (idx === entryChain.length - 1) {
                return (<Typography key={entry.displayName} color="text.primary">{entry.displayName}</Typography>);
            } else {
                return (<Link key={entry.displayName} underline="hover" color="inherit" onClick={() => onClickBreadcrumb(entry)}>{entry.displayName}</Link>);
            }
        });

        return (
            <Breadcrumbs className="mcc-breadcrumbs">
                <Link key="home" color="inherit" underline="hover" onClick={() => onClickBreadcrumb(null)}>Home</Link>
                {entryChainButtons}
            </Breadcrumbs>
        );
    };

    return (
        <Grid container={true} spacing={1}>
            <Grid container={true} item={true} md={12}>
                {preRenderBreadcrumb()}
            </Grid>
            <Grid container={true} item={true} md={12}>
                <ConfigEditor
                    ParentEntry={parentEntry}
                    Entries={entries}
                    Root={props.Root}
                    navigateToEntry={navigateToEntry}
                    IsReadOnly={props.IsReadOnly}
                />
            </Grid>
    </Grid>
    );
}

export default NavigableConfigEditor;
