/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { Input } from "reactstrap";
import Entry from "../../models/Entry";
import { InputEditorBasePropModel } from "./InputEditorBase";

interface SelectionStateModel {
    PossibleValues: string[];
}

export default class SelectionEditorBase extends React.Component<InputEditorBasePropModel, SelectionStateModel> {
    constructor(props: InputEditorBasePropModel) {
        super(props);

        if (this.props.Entry.value.possible != null) {
            const possibleValues: string[] = [...this.props.Entry.value.possible];
            if (possibleValues.find((value: string) => value === this.props.Entry.value.current) === undefined) {
                possibleValues.unshift("");
            }

            this.state = { PossibleValues: possibleValues };
        }
    }

    public onValueChange(e: React.FormEvent<HTMLInputElement>, entry: Entry): void {
        entry.value.current = e.currentTarget.value;
        this.setState({ PossibleValues: this.props.Entry.value.possible });
        this.forceUpdate();
    }

    public render(): React.ReactNode {
        return (
            <Input type="select" value={this.props.Entry.value.current !== null ? this.props.Entry.value.current : ""}
                   disabled={this.props.Entry.value.isReadOnly || this.props.IsReadOnly}
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
