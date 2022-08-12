/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { Button, ButtonGroup, Col, Collapse, Container, DropdownItem, DropdownMenu, DropdownToggle, Row, UncontrolledDropdown } from "reactstrap";
import Entry from "../../models/Entry";

export interface InputEditorBasePropModel {
    Entry: Entry;
    IsReadOnly: boolean;
}

interface InputEditorBaseStateModel {
}

export default class InputEditorBase extends React.Component<InputEditorBasePropModel, InputEditorBaseStateModel> {
    constructor(props: InputEditorBasePropModel) {
        super(props);
        this.state = { };
    }

    public onValueChange(e: React.FormEvent<HTMLInputElement>, entry: Entry): void {
        entry.value.current = e.currentTarget.value;
        this.forceUpdate();
    }
}
