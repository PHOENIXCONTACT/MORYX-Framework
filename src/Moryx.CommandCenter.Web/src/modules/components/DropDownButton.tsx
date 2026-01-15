/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiMenuDown } from "@mdi/js";
import Button from "@mui/material/Button";
import ButtonGroup from "@mui/material/ButtonGroup";
import ClickAwayListener from "@mui/material/ClickAwayListener";
import Grow from "@mui/material/Grow";
import MenuItem from "@mui/material/MenuItem";
import MenuList from "@mui/material/MenuList";
import Paper from "@mui/material/Paper";
import Popper from "@mui/material/Popper";
import SvgIcon from "@mui/material/SvgIcon";
import * as React from "react";

type OnClickFunction = () => void;

export interface ButtonConfig {
    Label: string;
    onClick: OnClickFunction;
}

export interface ConsoleMethodResultPropModel {
    Buttons: ButtonConfig[];
}

function DropDownButton(props: ConsoleMethodResultPropModel) {
    const [open, setOpen] = React.useState(false);
    const [selectedIndex, setSelectedIndex] = React.useState(0);
    const anchorRef = React.useRef<HTMLDivElement>(null);

    const onMenuItemClick = (event: React.MouseEvent<HTMLLIElement, MouseEvent>, index: number) => {
        setSelectedIndex(index);
        setOpen(false);
    };

    const onClose = (event: Event) => {
        if (
          anchorRef.current &&
          anchorRef.current.contains(event.target as HTMLElement)
        ) {
          return;
        }

        setOpen(false);
      };

    const onToggle = () => {
    setOpen((prevOpen) => !prevOpen);
    };

    return ([
        <ButtonGroup
            variant="contained"
            ref={anchorRef}
            sx={{minWidth: 200}}
        >
            <Button
                fullWidth={true}
                onClick={props.Buttons[selectedIndex].onClick}
            >
                {props.Buttons[selectedIndex].Label}</Button>
            <Button
                size="small"
                onClick={onToggle}
            >
            <SvgIcon><path d={mdiMenuDown} /></SvgIcon>
            </Button>
        </ButtonGroup>,
        <Popper
            sx={{ zIndex: 1 }}
            open={open}
            anchorEl={anchorRef.current}
            role={undefined}
            transition={true}
            disablePortal={true}
            placement="bottom"
        >
            {({ TransitionProps, placement }) => (
            <Grow
                {...TransitionProps}
                style={{
                    transformOrigin:
                      placement === "bottom" ? "center top" : "center bottom",
                  }}
            >
                <Paper>
                    <ClickAwayListener onClickAway={onClose}>
                        <MenuList sx={{minWidth: 200}} autoFocusItem={true}>
                        {props.Buttons.map((button, index) => (
                            <MenuItem
                                key={button.Label}
                                disabled={index === 2}
                                selected={index === selectedIndex}
                                onClick={(event) => onMenuItemClick(event, index)}
                            >
                                {button.Label}
                            </MenuItem>
                        ))}
                        </MenuList>
                    </ClickAwayListener>
                </Paper>
            </Grow>
            )}
        </Popper>
    ]);
}

export default DropDownButton;
