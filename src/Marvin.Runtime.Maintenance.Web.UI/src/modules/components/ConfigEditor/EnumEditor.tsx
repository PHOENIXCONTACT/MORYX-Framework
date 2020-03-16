/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { Input } from "reactstrap";
import Entry from "../../models/Entry";
import { InputEditorBasePropModel } from "./InputEditorBase";

interface EnumEditorStateModel {
    PossibleValues: string[];
}

export default class EnumEditor extends React.Component<InputEditorBasePropModel, EnumEditorStateModel> {
    constructor(props: InputEditorBasePropModel) {
        super(props);

        const possibleValues: string[] = [...this.props.Entry.Value.Possible];
        if (possibleValues.find((value: string) => value === this.props.Entry.Value.Current) === undefined) {
            possibleValues.unshift("");
        }

        this.state = { PossibleValues: possibleValues };
    }

    public onValueChange(e: React.FormEvent<HTMLInputElement>, entry: Entry): void {
        entry.Value.Current = e.currentTarget.value;
        this.setState({ PossibleValues: this.props.Entry.Value.Possible });
        this.forceUpdate();
    }

    public render(): React.ReactNode {
        return (
            <Input type="select" value={this.props.Entry.Value.Current !== null ? this.props.Entry.Value.Current : ""}
                   disabled={this.props.Entry.Value.IsReadOnly || this.props.IsReadOnly}
                   onChange={(e: React.FormEvent<HTMLInputElement>) => this.onValueChange(e, this.props.Entry)}>
                {
                    this.state.PossibleValues.map((possibleValue, idx) => {
                        return (<option key={idx} value={possibleValue}>{possibleValue}</option>);
                    })
                }
            </Input>
        );
    }
}
