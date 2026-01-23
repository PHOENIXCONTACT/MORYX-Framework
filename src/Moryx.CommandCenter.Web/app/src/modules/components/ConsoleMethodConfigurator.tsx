/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Button from "@mui/material/Button";
import Grid from "@mui/material/Grid";
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
    <Grid container={true} spacing={1}>
      {props.Method == null ? (
        <Grid size={12}>
          <Typography variant="body2">Please select a method.</Typography>
        </Grid>
      ) : [

        <Grid container={true} size={12} key="name">
          <Grid size={3}><Typography variant="body2" fontWeight="bold">Name:</Typography></Grid>
          <Grid size={9}><Typography variant="body2">{props.Method.displayName}</Typography></Grid>
        </Grid>,
        <Grid container={true} size={12} key="description">
          <Grid size={3}><Typography variant="body2" fontWeight="bold">Description:</Typography></Grid>
          <Grid size={9}><Typography variant="body2">{props.Method.description}</Typography></Grid>
        </Grid>,
        <Grid size={12} key="params">
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
        </Grid>,
        <Grid size={12} key="invoke" sx={{ display: "flex", justifyContent: "flex-end" }}>
          <Button variant="contained" onClick={invokeSelectedMethod}>
            Invoke
          </Button>
        </Grid>
      ]}
    </Grid>
  );
}

export default ConsoleMethodConfigurator;
