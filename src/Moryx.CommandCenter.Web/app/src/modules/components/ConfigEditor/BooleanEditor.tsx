/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import FormControlLabel from "@mui/material/FormControlLabel";
import Switch from "@mui/material/Switch";
import Tooltip from "@mui/material/Tooltip";
import * as React from "react";
import InputEditorBase, { InputEditorBasePropModel } from "./InputEditorBase";

export default class ByteEditor extends InputEditorBase {
  constructor(props: InputEditorBasePropModel) {
    super(props);
    this.state = {};
  }

  public render(): React.ReactNode {
    return (

      <Tooltip title={this.props.Entry.description} placement="right">
        <FormControlLabel control={
          <Switch defaultChecked={this.props.Entry.value.current.toLowerCase() === "true"}
                  disabled={(this.props.Entry.value.isReadOnly || this.props.IsReadOnly)}
                  onClick={(e: React.MouseEvent<HTMLElement>) => this.onToggle(e)}

          />} label={this.props.Entry.displayName}/>
      </Tooltip>
    );
  }

  private onToggle(e: React.MouseEvent<HTMLElement>): void {
    this.props.Entry.value.current = this.props.Entry.value.current === "True" ? this.props.Entry.value.current = "False" : this.props.Entry.value.current = "True";
    this.forceUpdate();
  }
}
