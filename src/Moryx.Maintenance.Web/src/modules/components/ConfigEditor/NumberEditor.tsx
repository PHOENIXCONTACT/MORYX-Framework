/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { Input } from "reactstrap";
import { toString } from "../../models/EntryValueType";
import { InputEditorBasePropModel } from "./InputEditorBase";
import SelectionEditorBase from "./SelectionEditorBase";

export default class NumberEditor extends SelectionEditorBase {
    constructor(props: InputEditorBasePropModel) {
        super(props);
    }

    private preRenderInput(): React.ReactNode {
        return (<Input type="number"
                onChange={(e: React.FormEvent<HTMLInputElement>) => this.onValueChange(e, this.props.Entry)}
                placeholder={"Please enter a value of type: " + toString(this.props.Entry.Value.Type) + " ..."}
                disabled={this.props.Entry.Value.IsReadOnly || this.props.IsReadOnly}
                value={this.props.Entry.Value.Current}
        />);
    }

    public render(): React.ReactNode {
        return this.props.Entry.Value.Possible != null && this.props.Entry.Value.Possible.length > 0 ?
                super.render() : this.preRenderInput();
    }
}
