/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import queryString from "query-string";
import * as React from "react";
import { Location, useLocation, useNavigate } from "react-router-dom";
import { Button, ButtonGroup, Col, Container, Row } from "reactstrap";
import Entry from "../../models/Entry";
import ConfigEditor from "./ConfigEditor";

interface NavigableConfigEditorPropModel {
    ParentEntry: Entry;
    Entries: Entry[];
    IsReadOnly: boolean;
    Root: Entry;
}

interface NavigableConfigEditorStateModel {
    EntryChain: Entry[];
    ParentEntry: Entry;
    Entries: Entry[];
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
        const entryChainButtons = entryChain.map((entry, idx) => (
            <Button key={idx} color="light" onClick={() => onClickBreadcrumb(entry)} disabled={idx === entryChain.length - 1} >{entry.displayName}</Button>
        ));

        return (
            <ButtonGroup>
                <Button color="dark" disabled={entryChain.length === 0} onClick={() => onClickBreadcrumb(null)}>Home</Button>
                {entryChainButtons}
            </ButtonGroup>
        );
    };

    return (
        <div>
            {preRenderBreadcrumb()}
            <Container fluid={true} className="up-space-lg no-padding">
                <Row className="config-editor-header">
                    <Col md={5} className="no-padding"><span className="font-bold">Property</span></Col>
                    <Col md={7} className="no-padding"><span className="font-bold">Value</span></Col>
                </Row>
                <ConfigEditor
                    ParentEntry={parentEntry}
                    Entries={entries}
                    Root={props.Root}
                    navigateToEntry={navigateToEntry}
                    IsReadOnly={props.IsReadOnly}
                />
            </Container>
        </div>
    );
}

export default NavigableConfigEditor;
