/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiHexadecimal } from "@mdi/js";
import InputAdornment from "@mui/material/InputAdornment";
import MenuItem from "@mui/material/MenuItem";
import SvgIcon from "@mui/material/SvgIcon";
import TextField from "@mui/material/TextField";
import Tooltip from "@mui/material/Tooltip";
import * as React from "react";
import InputEditorBase, { InputEditorBasePropModel } from "./InputEditorBase";

export default class ByteEditor extends InputEditorBase {
  constructor(props: InputEditorBasePropModel) {
    super(props);
    this.state = {};
  }

  private static preRenderOptions(): React.ReactNode {
    const options: React.ReactNode[] = [];
    for (let idx = 0; idx < 256; ++idx) {
      options.push(<MenuItem key={idx} value={idx}>{"0x" + idx.toString(16).toUpperCase()}</MenuItem>);
    }
    return options;
  }

  public render(): React.ReactNode {
    return (
      <Tooltip title={this.props.Entry.description} placement="right">
        <TextField value={this.props.Entry.value.current}
                   label={this.props.Entry.displayName}
                   disabled={this.props.Entry.value.isReadOnly || this.props.IsReadOnly}
                   onChange={(e) => this.onValueChange(e.target.value, this.props.Entry)}
                   fullWidth={true}
                   size="small"
                   margin="dense"
                   slotProps={{
                     input: {
                       endAdornment: (
                         <InputAdornment position="end">
                           <SvgIcon><path d={mdiHexadecimal} /></SvgIcon>
                         </InputAdornment>
                       ),
                     },
                   }}>
          {ByteEditor.preRenderOptions()}
        </TextField>
      </Tooltip>
    );
  }
}
