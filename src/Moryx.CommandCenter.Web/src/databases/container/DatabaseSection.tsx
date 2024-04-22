/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import * as React from "react";

interface DatabaseSectionPropsModel {
    title: React.ReactNode | string;
    width?: number;
}

export class DatabaseSection extends React.Component<React.PropsWithChildren<DatabaseSectionPropsModel>> {

    constructor(props: React.PropsWithChildren<DatabaseSectionPropsModel>) {
        super(props);
    }

    public render(): React.ReactNode {
        const width = this.props.width ?? 12;
        return (
            <Grid container={true} item={true} md={width}>
                <Grid container={true} item={true} md={12}>
                    {(typeof this.props.title === "string")
                        ? <Typography variant="h5" gutterBottom={true}>{this.props.title}</Typography>
                        : this.props.title
                    }

                </Grid>
                <Grid container={true} item={true} md={12} spacing={1}>
                    {this.props.children}
                </Grid>
            </Grid>
        );
    }
}
