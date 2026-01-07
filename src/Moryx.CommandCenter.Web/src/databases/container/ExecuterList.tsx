/*
 * Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import IconButton from "@mui/material/IconButton";
import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ListItemText from "@mui/material/ListItemText";
import SvgIcon from "@mui/material/SvgIcon";
import * as React from "react";

type OnExecuteFunction = () => void;

export interface ExecuterItem {
    title: string;
    subtitle: string;
    icon: string;
    enabled: boolean;
    hidden: boolean;
    onExecute: OnExecuteFunction;
}

interface ExecuterListPropsModel {
    items: ExecuterItem[];
}

export class ExecuterList extends React.Component<ExecuterListPropsModel> {

    constructor(props: ExecuterListPropsModel) {
        super(props);
    }

    public render(): React.ReactNode {
        const items = this.props.items;
        return (
            <List dense={true}>
            {
                items.map((item, idx) => {
                    return (<ListItem
                        key={idx}
                        divider={idx < items.length - 1}
                        secondaryAction={
                            (item.hidden
                                ? null
                                : <IconButton
                                    edge="end"
                                    aria-label="Execute"
                                    onClick={item.onExecute}
                                    disabled={!item.enabled}
                                    color="primary">
                                    <SvgIcon><path d={item.icon} /></SvgIcon>
                                </IconButton>)
                        }>
                        <ListItemText
                            primary={item.title}
                            secondary={item.subtitle} />
                    </ListItem>);
                })
            }
            </List>
        );
    }
}
