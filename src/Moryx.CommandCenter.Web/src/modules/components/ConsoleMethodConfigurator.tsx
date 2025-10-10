/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Button from "@mui/material/Button";
import GridLegacy from "@mui/material/GridLegacy";
import Typography from "@mui/material/Typography";
import * as React from "react";
import MethodEntry from "../models/MethodEntry";
import NavigableConfigEditor from "./ConfigEditor/NavigableConfigEditor";

export interface ConsoleMethodConfiguratorPropModel {
    ModuleName: string;
    Method: MethodEntry;
    onInvokeMethod(methodEntry: MethodEntry): void;
}

function ConsoleMethodConfigurator(props: ConsoleMethodConfiguratorPropModel) {
    const invokeSelectedMethod = (): void => {
        props.onInvokeMethod(props.Method);
    };

    return (
        <GridLegacy container={true} spacing={1}>
            {props.Method == null ? (
                <GridLegacy item={true} md={12}>
                    <Typography variant="body2">Please select a method.</Typography>
                </GridLegacy>
            ) : [

                <GridLegacy container={true} item={true}>
                    <GridLegacy item={true} md={3}><Typography variant="body2" fontWeight="bold">Name:</Typography></GridLegacy>
                    <GridLegacy item={true} md={9}><Typography variant="body2">{props.Method.displayName}</Typography></GridLegacy>
                </GridLegacy>,
                <GridLegacy container={true} item={true}>
                    <GridLegacy item={true} md={3}><Typography variant="body2" fontWeight="bold">Description:</Typography></GridLegacy>
                    <GridLegacy item={true} md={9}><Typography variant="body2">{props.Method.description}</Typography></GridLegacy>
                </GridLegacy>,
                <GridLegacy container={true} item={true}>
                    <GridLegacy item={true} md={12}>
                        {props.Method.parameters.subEntries.length === 0 ? (
                            <Typography variant="body2">This method is parameterless.</Typography>
                        ) : (
                            <NavigableConfigEditor
                                Entries={props.Method.parameters.subEntries}
                                ParentEntry={null}
                                Root={props.Method.parameters}
                                IsReadOnly={false}
                            />
                        )}
                    </GridLegacy>
                </GridLegacy>,
                <GridLegacy container={true} item={true}>
                    <GridLegacy container={true} item={true} md={12} direction="row" justifyContent="flex-end">
                        <Button variant="contained" onClick={invokeSelectedMethod}>
                            Invoke
                        </Button>
                    </GridLegacy>
                </GridLegacy>
            ]}
        </GridLegacy>
    );
}

export default ConsoleMethodConfigurator;
