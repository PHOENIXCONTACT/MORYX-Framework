/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiFormatText } from "@mdi/js";
import InputAdornment from "@mui/material/InputAdornment";
import SvgIcon from "@mui/material/SvgIcon";
import TextField from "@mui/material/TextField";
import Tooltip from "@mui/material/Tooltip";
import * as React from "react";
import { InputEditorBasePropModel } from "./InputEditorBase";
import SelectionEditorBase from "./SelectionEditorBase";

export default class StringEditor extends SelectionEditorBase {
  constructor(props: InputEditorBasePropModel) {
    super(props);
  }

  private preRenderInput(): React.ReactNode {
    // LoadError is handled here, which is not the right place and should be
    // Changed inf the future.
    const isLoadError = this.props.Entry.displayName === "LoadError";
    const currentValue = this.props.Entry.value.current;

    return (
      isLoadError && currentValue == null
        ? null
        : <Tooltip title={this.props.Entry.description} placement="right">
          <TextField type={this.props.Entry.validation.isPassword ? "password" : "text"}
                     onChange={(e) => this.onValueChange(e.target.value, this.props.Entry)}
                     label={this.props.Entry.displayName}
                     aria-label={this.props.Entry.description}
                     fullWidth={true}
                     error={isLoadError}
                     multiline={isLoadError}
                     rows={4}
                     disabled={(this.props.Entry.value.isReadOnly || this.props.IsReadOnly) && !isLoadError}
                     value={currentValue == null ? "" : currentValue}
                     size="small"
                     margin="dense"
                     slotProps={{
                       input: {
                         endAdornment: (
                           <InputAdornment position="end">
                             <SvgIcon><path d={mdiFormatText} /></SvgIcon>
                           </InputAdornment>
                         ),
                       },
                     }}/>
        </Tooltip>);
  }

  private preRenderPossibleValueList(): React.ReactNode {
    this.state = {PossibleValues: this.props.Entry.value.possible};
    return super.render();
  }

  public render(): React.ReactNode {
    return this.props.Entry.value.possible != null && this.props.Entry.value.possible.length > 0
      ? this.preRenderPossibleValueList()
      : this.preRenderInput();
  }
}
