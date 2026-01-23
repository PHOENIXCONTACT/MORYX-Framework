/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import * as React from "react";

interface ModuleInfoPropModel {
  title: string;
  spacing?: number;
  md?: number;
}

export class ModuleInfoTile extends React.Component<React.PropsWithChildren<ModuleInfoPropModel>> {

  constructor(props: ModuleInfoPropModel) {
    super(props);

  }

  public render() {
    return (
      <Grid size={this.props.md ?? 6}>
        <Grid size={12}>
          <Typography variant="h5" gutterBottom={true}>{this.props.title}</Typography>
        </Grid>

        <Grid container={true} size={12}
              direction="column"
              justifyContent="flex-start"
              alignItems="stretch"
              spacing={this.props.spacing != null ? this.props.spacing : 0}
        >
          {this.props.children}
        </Grid>
      </Grid>
    );
  }
}
