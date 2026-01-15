/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { connect } from "react-redux";
import { AppState } from "../redux/AppState";

interface ClockPropModel {
    Time?: string;
}

const mapStateToProps = (state: AppState): ClockPropModel => {
    return {
      Time: state.Common.ServerTime,
    };
};

export class Clock extends React.Component<ClockPropModel> {
    constructor(props: ClockPropModel) {
        super(props);
    }

    public render(): React.ReactNode {
        return (
            <span>{this.props.Time}</span>
        );
    }
}

export default connect<ClockPropModel>(mapStateToProps)(Clock);
