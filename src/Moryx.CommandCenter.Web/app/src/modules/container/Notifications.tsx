/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Table from "@mui/material/Table";
import TableBody from "@mui/material/TableBody";
import TableCell from "@mui/material/TableCell";
import TableContainer from "@mui/material/TableContainer";
import TableFooter from "@mui/material/TableFooter";
import TableHead from "@mui/material/TableHead";
import TablePagination from "@mui/material/TablePagination";
import TableRow from "@mui/material/TableRow";
import * as React from "react";
import NotificationModel from "../models/NotificationModel";
import { NotificationRow } from "./NotificationRow";

interface NotificationsPropModel {
    messages: NotificationModel[];
}

interface NotificationPaginationState {
    page: number;
    rowsPerPage: number;
}

const defaultPageIndex: number = 0;
const defaultRowsPerPage: number = 25;

export class Notifications extends React.Component<NotificationsPropModel, NotificationPaginationState> {

    constructor(props: NotificationsPropModel) {
        super(props);
        this.state = {
            page: defaultPageIndex,
            rowsPerPage: defaultRowsPerPage
        };
    }

    public render() {
        const messages = this.props.messages;
        const { page, rowsPerPage } = this.state;

        const paginatedMessages = messages.slice(
            page * rowsPerPage,
            page * rowsPerPage + rowsPerPage
        );

        const handlePageChange = (_event: unknown, newPage: number): void => {
            this.setState({ page: newPage });
        };

        const getCurrentPage = (currentPageIndex: number, totalPageNum: number): number => {
            return currentPageIndex > totalPageNum - 1 ? totalPageNum - 1 : currentPageIndex;
        };

        const handleChangeRowsPerPage = (
            event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
        ): void => {
            const newRowsPerPage = parseInt(event.target.value, 10);
            this.setState({
                rowsPerPage: newRowsPerPage,
                page: getCurrentPage(this.state.page, Math.ceil(messages.length / newRowsPerPage))
            });
        };

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
                        { (paginatedMessages.length > 0)
                            ? (paginatedMessages.map((message, idx) => (
                                <NotificationRow message={message}/>
                            )))
                            : <TableRow>
                                <TableCell align="center" colSpan={4}>No notifications</TableCell>
                            </TableRow>
                        }
                    </TableBody>
                    <TableFooter>
                        <TablePagination
                            count={messages.length}
                            page={page}
                            onPageChange={handlePageChange}
                            rowsPerPage={rowsPerPage}
                            onRowsPerPageChange={handleChangeRowsPerPage}
                            rowsPerPageOptions={[25, 50, 100]}
                        />
                    </TableFooter>
                </Table >
            </TableContainer >
        );
    }
}
