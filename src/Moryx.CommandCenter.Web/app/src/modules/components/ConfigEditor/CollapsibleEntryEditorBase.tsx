/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Collapse from "@mui/material/Collapse";
import * as React from "react";
import Entry from "../../models/Entry";

export interface CollapsibleEntryEditorBasePropModel extends React.PropsWithChildren {
  Entry: Entry;
  IsExpanded: boolean;
  IsReadOnly: boolean;
  Root: Entry;

  navigateToEntry(entry: Entry): void;
}

export default class CollapsibleEntryEditorBase<T> extends React.Component<CollapsibleEntryEditorBasePropModel, T> {
  constructor(props: CollapsibleEntryEditorBasePropModel) {
    super(props);
  }

  public render(): React.ReactNode {
    return (
      <div>
        <Collapse in={this.props.IsExpanded}>
          {this.props.children}
        </Collapse>
      </div>
    );
  }
}
