/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { Input } from "reactstrap";
import { InputEditorBasePropModel } from "./InputEditorBase";
import SelectionEditorBase from "./SelectionEditorBase";

export default class StringEditor extends SelectionEditorBase {
    constructor(props: InputEditorBasePropModel) {
        super(props);
    }

    private preRenderInput(): React.ReactNode {
        return (<Input type={this.props.Entry.Validation.IsPassword ? "password" : "text"}
                        onChange={(e: React.FormEvent<HTMLInputElement>) => this.onValueChange(e, this.props.Entry)}
                        placeholder={"Please enter a string ..."}
                        disabled={this.props.Entry.Value.IsReadOnly || this.props.IsReadOnly}
                        value={this.props.Entry.Value.Current == null ? "" : this.props.Entry.Value.Current} />);
    }

    private preRenderPossibleValueList(): React.ReactNode {
        return super.render();
    }

    public render(): React.ReactNode {
        return this.props.Entry.Value.Possible != null && this.props.Entry.Value.Possible.length > 0 ?
                this.preRenderPossibleValueList() : this.preRenderInput();
    }
}
