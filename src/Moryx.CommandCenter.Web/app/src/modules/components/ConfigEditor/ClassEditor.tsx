/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Collapse from "@mui/material/Collapse";
import Container from "@mui/material/Container";
import * as React from "react";
import CollapsibleEntryEditorBase, { CollapsibleEntryEditorBasePropModel } from "./CollapsibleEntryEditorBase";
import ConfigEditor from "./ConfigEditor";

interface ClassEditorStateModel {
}

export default class ClassEditor extends CollapsibleEntryEditorBase<ClassEditorStateModel> {
  constructor(props: CollapsibleEntryEditorBasePropModel) {
    super(props);
    this.state = {};
  }

  public preRenderConfigEditor(): React.ReactNode {
    return <ConfigEditor ParentEntry={this.props.Entry}
                         Entries={this.props.Entry.subEntries}
                         Root={this.props.Root}
                         navigateToEntry={this.props.navigateToEntry}
                         IsReadOnly={this.props.IsReadOnly}/> as React.ReactNode;
  }

  public render(): React.ReactNode {
    return (
      <div>
        <Collapse in={this.props.IsExpanded}>
          {
            this.props.IsExpanded &&
            <Container style={{paddingRight: "0"}}>
              {this.preRenderConfigEditor()}
            </Container>
          }
        </Collapse>
      </div>
    );
  }
}
