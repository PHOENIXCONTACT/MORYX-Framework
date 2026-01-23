/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiNumeric } from "@mdi/js";
import InputAdornment from "@mui/material/InputAdornment";
import SvgIcon from "@mui/material/SvgIcon";
import TextField from "@mui/material/TextField";
import Tooltip from "@mui/material/Tooltip";
import * as React from "react";
import { InputEditorBasePropModel } from "./InputEditorBase";
import SelectionEditorBase from "./SelectionEditorBase";

export default class NumberEditor extends SelectionEditorBase {
  constructor(props: InputEditorBasePropModel) {
    super(props);
  }

  private preRenderInput(): React.ReactNode {
    const tooltip = this.props.Entry.description + " - Please enter a value of type: " + this.props.Entry.value.type;
    return (
      <Tooltip title={tooltip} placement="right">
        <TextField
          type="number"
          label={this.props.Entry.displayName}
          value={Number(this.props.Entry.value.current)}
          onChange={(e) => this.onValueChange(e.target.value, this.props.Entry)}
          disabled={this.props.Entry.value.isReadOnly || this.props.IsReadOnly}
          fullWidth={true}
          size="small"
          margin="dense"
          slotProps={{
            input: {
              endAdornment: (
                <InputAdornment position="end">
                  <SvgIcon><path d={mdiNumeric} /></SvgIcon>
                </InputAdornment>
              ),
            },
          }}
        />
      </Tooltip>);
  }

  public render(): React.ReactNode {
    return this.props.Entry.value.possible != null && this.props.Entry.value.possible.length > 0 ?
      super.render() : this.preRenderInput();
  }
}
