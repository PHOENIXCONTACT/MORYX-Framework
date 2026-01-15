/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
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

    public onValueChange(e: string, entry: Entry): void {
        entry.value.current = e;
        this.forceUpdate();
    }
}
