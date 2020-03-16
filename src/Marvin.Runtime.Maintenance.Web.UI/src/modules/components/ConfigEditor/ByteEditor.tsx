/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { Input } from "reactstrap";
import InputEditorBase, { InputEditorBasePropModel } from "./InputEditorBase";

export default class ByteEditor extends InputEditorBase {
    constructor(props: InputEditorBasePropModel) {
        super(props);
        this.state = { };
    }

    private static preRenderOptions(): React.ReactNode {
        const options: React.ReactNode[] = [];
        for (let idx = 0; idx < 256; ++idx) {
            options.push(<option key={idx} value={idx}>{"0x" + idx.toString(16).toUpperCase()}</option>);
        }
        return options;
    }

    public render(): React.ReactNode {
        return (
            <Input type="select" value={this.props.Entry.Value.Current}
                   disabled={this.props.Entry.Value.IsReadOnly || this.props.IsReadOnly}
                   onChange={(e: React.FormEvent<HTMLInputElement>) => this.onValueChange(e, this.props.Entry)}>
                   {ByteEditor.preRenderOptions()}
            </Input>
        );
    }
}
