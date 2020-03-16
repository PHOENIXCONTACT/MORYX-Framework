/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { Input } from "reactstrap";
import { toString } from "../../models/EntryValueType";
import InputEditorBase, { InputEditorBasePropModel } from "./InputEditorBase";

export default class NumberEditor extends InputEditorBase {
    constructor(props: InputEditorBasePropModel) {
        super(props);
        this.state = { };
    }

    public render(): React.ReactNode {
        return (<Input type="number"
                        onChange={(e: React.FormEvent<HTMLInputElement>) => this.onValueChange(e, this.props.Entry)}
                        placeholder={"Please enter a value of type: " + toString(this.props.Entry.Value.Type) + " ..."}
                        disabled={this.props.Entry.Value.IsReadOnly || this.props.IsReadOnly}
                        value={this.props.Entry.Value.Current}
                />);
    }
}
