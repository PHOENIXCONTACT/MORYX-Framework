/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Grid from "@mui/material/Grid";
import SvgIcon from "@mui/material/SvgIcon";
import Typography from "@mui/material/Typography";
import * as React from "react";

interface SectionInfoPropModel {
  description: string;
  icon: string;
}

export class SectionInfo extends React.Component<SectionInfoPropModel> {
  constructor(props: SectionInfoPropModel) {
    super(props);
  }

  public render(): React.ReactNode {
    return (
      <Grid container={true}
            direction="row"
            justifyContent="center"
            alignItems="center"
            spacing={1}
      >
        <Grid>
          <SvgIcon>
            <path d={this.props.icon}/>
          </SvgIcon>
        </Grid>
        <Grid>
          <Typography variant="body2">{this.props.description}</Typography>
        </Grid>
      </Grid>
    );
  }
}
