/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableHead from "@mui/material/TableHead";
import TableRow from "@mui/material/TableRow";
import * as React from "react";
import NotificationModel from "../models/NotificationModel";
import { NotificationRow } from "./NotificationRow";

interface NotificationsPropModel {
    messages: NotificationModel[];
}

export class Notifications extends React.Component<NotificationsPropModel> {

    constructor(props: NotificationsPropModel) {
        super(props);

    }

    public render() {
        const messages = this.props.messages;
        return (
            <TableContainer>
                <Table
                    size={"small"}
                >
                    <TableHead >
                        <TableRow>
                            <TableCell/>
                            <TableCell align="left"/>
                            <TableCell align="left">Message</TableCell>
                            <TableCell align="left" sx={{paddingLeft: 0, paddingRight: 0}}>Type</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        { (messages.length > 0)
                            ? (messages.map((message, idx) => (

                                <NotificationRow message={message}/>
                            )))
                            : <TableRow>
                                <TableCell align="center" colSpan={4}>No notifications</TableCell>
                            </TableRow>
                        }
                    </TableBody>
                </Table >
            </TableContainer >
        );

    }
}
