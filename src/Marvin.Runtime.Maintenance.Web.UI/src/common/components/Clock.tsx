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
