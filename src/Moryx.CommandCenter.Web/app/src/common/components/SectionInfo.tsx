/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import GridLegacy from "@mui/material/GridLegacy";
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
            <GridLegacy container={true}
                direction="row"
                justifyContent="center"
                alignItems="center"
                spacing={1}
            >
                <GridLegacy item={true}>
                    <SvgIcon>
                        <path d={this.props.icon} />
                    </SvgIcon>
                </GridLegacy>
                <GridLegacy item={true}>
                    <Typography variant="body2">{this.props.description}</Typography>
                </GridLegacy>
            </GridLegacy>
        );
    }
}
