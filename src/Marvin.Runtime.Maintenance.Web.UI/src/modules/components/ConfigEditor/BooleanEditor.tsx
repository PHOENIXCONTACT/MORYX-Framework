/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import "bootstrap-toggle/css/bootstrap2-toggle.css";
import * as React from "react";
import BootstrapToggle from "react-bootstrap-toggle";
import { Input } from "reactstrap";
import Entry from "../../models/Entry";
import InputEditorBase, { InputEditorBasePropModel } from "./InputEditorBase";

export default class ByteEditor extends InputEditorBase {
    constructor(props: InputEditorBasePropModel) {
        super(props);
        this.state = { };
    }

    public render(): React.ReactNode {
        return (
            <BootstrapToggle active={this.props.Entry.Value.Current.toLowerCase() === "true"}
                             disabled={this.props.Entry.Value.IsReadOnly || this.props.IsReadOnly}
                             onClick={(e: React.MouseEvent<HTMLElement>) => this.onToggle(e)}
                             height="35px" />
        );
    }

    private onToggle(e: React.MouseEvent<HTMLElement>): void {
        this.props.Entry.Value.Current = this.props.Entry.Value.Current === "True" ? this.props.Entry.Value.Current = "False" : this.props.Entry.Value.Current = "True";
        this.forceUpdate();
    }
}
