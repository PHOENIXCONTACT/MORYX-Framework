/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Chip from "@mui/material/Chip";
import * as React from "react";
import { getEnumTypeValue } from "../converter/EnumTypeHelper";
import { ModuleServerModuleState } from "../models/ModuleServerModuleState";
import { HealthStateToCssClassConverter } from "./HealthStateToCssClassConverter";

interface HealthStateBadgeProps {
    HealthState: ModuleServerModuleState;
}

export class HealthStateBadge extends React.Component<HealthStateBadgeProps> {
    constructor(props: HealthStateBadgeProps) {
        super(props);
    }

    public render(): React.ReactNode {
        const text = getEnumTypeValue(ModuleServerModuleState, this.props.HealthState);
        const color = HealthStateToCssClassConverter.Color(text);

        return (<Chip sx={{fontSize: "0.6rem", height: "1rem"}} color={color} size="small" label={text}/>);
    }
}
