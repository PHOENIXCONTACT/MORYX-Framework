/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Button from "@mui/material/Button";
import Container from "@mui/material/Container";
import Grid from "@mui/material/Grid";
import * as React from "react";
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
                <Container>
                    <Grid container={true}>
                        <Grid item={true} md={3}><span className="font-bold">Name:</span></Grid>
                        <Grid item={true} md={9}><span className="font-italic">{props.Method.displayName}</span></Grid>
                        <Grid item={true} md={3}><span className="font-bold">Description:</span></Grid>
                        <Grid item={true} md={9}><span className="font-italic">{props.Method.description}</span></Grid>
                        <Grid item={true} md={12} className="up-space-lg">
                            <NavigableConfigEditor
                                Entries={props.InvokeResult.subEntries}
                                ParentEntry={null}
                                Root={props.InvokeResult}
                                IsReadOnly={true} />
                        </Grid>
                        <Grid container={true} item={true} md={12} direction="row" justifyContent="flex-end">
                            <Button variant="contained" className="float-right" onClick={resetInvokeResult}>
                                Reset result
                            </Button>
                        </Grid>
                    </Grid>
                </Container>
            )}
        </div>
    );
}

export default ConsoleMethodResult;
