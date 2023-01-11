/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { Badge } from "reactstrap";
import { ModuleServerModuleState } from "../../modules/models/ModuleServerModuleState";
import { HealthStateToCssClassConverter } from "../converter/HealthStateToCssClassConverter";

interface HealthStateBadgeProps {
    HealthState: ModuleServerModuleState;
}

export class HealthStateBadge extends React.Component<HealthStateBadgeProps> {
    constructor(props: HealthStateBadgeProps) {
        super(props);
    }

    public render(): React.ReactNode {
        const text = ModuleServerModuleState[this.props.HealthState];
        const color = HealthStateToCssClassConverter.Convert(this.props.HealthState);

        return (<Badge color={color.Background}>{text}</Badge>);
    }
}
