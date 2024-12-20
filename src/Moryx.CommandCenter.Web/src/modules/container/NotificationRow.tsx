/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiChevronDown, mdiChevronUp } from "@mdi/js";
import Collapse from "@mui/material/Collapse";
import Container from "@mui/material/Container";
import Grid from "@mui/material/Grid";
import IconButton from "@mui/material/IconButton";
import SvgIcon from "@mui/material/SvgIcon";
import TableCell from "@mui/material/TableCell";
import TableRow from "@mui/material/TableRow";
import Typography from "@mui/material/Typography";
import * as React from "react";
import { ModuleNotificationTypeToCssClassConverter } from "../converter/ModuleNotificationTypeToCssClassConverter";
import NotificationModel from "../models/NotificationModel";
import SerializableException from "../models/SerializableException";
import { Serverity } from "../models/Severity";

interface NotificationRowPropModel {
    message: NotificationModel;
}

interface NotificationRowStateModel {
    open: boolean;
}

export class NotificationRow extends React.Component<NotificationRowPropModel, NotificationRowStateModel> {

    constructor(props: NotificationRowPropModel) {
        super(props);
        this.state = {
            open: false,
        };
    }

    private static formatTimestamp(timestamp: Date): string {
        const d2 = function (n: number, digits = 2): string {
            return String(n).padStart(digits, "0");
        };
        return timestamp.getFullYear() + "-" +
            d2(timestamp.getMonth()) + "-" +
            d2(timestamp.getDate()) + " " +
            d2(timestamp.getHours()) + ":" +
            d2(timestamp.getMinutes()) + ":" +
            d2(timestamp.getSeconds()) + "." +
            timestamp.getMilliseconds();
    }

    public render() {
        const message = this.props.message;
        return (
            [
            <TableRow
                sx={{ "&:last-child td, &:last-child th": { border: 0 }, "& > td": { borderBottom: "none" }}}
            >
                <TableCell padding="none" width={48}>
                    <IconButton
                        size="small"
                        style={{...ModuleNotificationTypeToCssClassConverter.Convert(message.severity)}}
                        onClick={() => this.setState({open: !this.state.open})}>
                        <SvgIcon><path d={this.state.open ? mdiChevronUp : mdiChevronDown } /></SvgIcon>
                    </IconButton>
                </TableCell>
                <TableCell width={148} sx={{borderBottom: "none", maxWidth: 150, paddingLeft: 0, paddingRight: 0}}>
                    {<p style={{fontSize: 12, margin: "0px"}}>{NotificationRow.formatTimestamp(new Date(message.timestamp.toString()))}</p>}
                    {<p style={{fontSize: 12, margin: "0px", ...ModuleNotificationTypeToCssClassConverter.Convert(message.severity)}}>{Serverity[message.severity]}</p>}
                </TableCell>
                <TableCell
                    sx={{textOverflow: "ellipsis", overflow: "hidden", whiteSpace: "nowrap", maxWidth: "1px"}}
                >
                    {message.message}
                </TableCell>
                <TableCell
                    width={200}
                    sx={{paddingLeft: 0, paddingRight: 0, textOverflow: "ellipsis", overflow: "hidden", whiteSpace: "nowrap", maxWidth: "200px"}}
                >
                    {message.exception?.exceptionTypeName}
                </TableCell>
            </TableRow > ,
            <TableRow>
                <TableCell padding="none"/>
                <TableCell colSpan={3} padding="none" >
                    <Collapse in={this.state.open} unmountOnExit={true}>
                        <Container disableGutters={true}>
                            <Grid container={true} direction="row" rowSpacing={0.5} sx={{paddingBottom: 1}}>
                                <Grid container={true} item={true}>
                                    <Grid item={true} md={12}>
                                        <Typography variant="body2">{this.props.message.message}</Typography>
                                    </Grid>
                                </Grid>
                                <Grid container={true} item={true}>
                                    <Grid item={true} md={2}>
                                        <Typography variant="body2" sx={{ fontWeight: "bold"}}>Type</Typography>
                                    </Grid>
                                    <Grid item={true} md={10}>
                                        <Typography variant="body2" sx={{ fontFamily: "Monospace" }}>{this.props.message.exception?.exceptionTypeName}</Typography>
                                    </Grid>
                                </Grid>
                                <Grid container={true} item={true}>
                                    <Grid item={true} md={2}><Typography variant="body2" sx={{ fontWeight: "bold"}}>Message</Typography></Grid>
                                    <Grid item={true} md={10}> <Typography variant="body2" sx={{ fontFamily: "Monospace" }}>{this.props.message.exception?.message}</Typography></Grid>
                                </Grid>
                                <Grid container={true} item={true}>
                                    <Grid item={true} md={2}>
                                        <Typography variant="body2" sx={{ fontWeight: "bold"}}>Stack trace</Typography>
                                    </Grid>
                                    <Grid item={true} md={10}>
                                        <Typography variant="body2" sx={{ fontFamily: "Monospace" }}>{this.props.message.exception?.stackTrace}</Typography>
                                    </Grid>
                                </Grid>
                                <Grid container={true} item={true}>
                                    <Grid item={true} md={12}>
                                        {this.props.message.exception?.innerException == null ? (
                                            <Typography variant="body2">No inner exception found</Typography>
                                        ) : (
                                            <Typography variant="body2" sx={{ fontWeight: "bold"}}>Inner exception</Typography>
                                        )}
                                    </Grid>
                                </Grid>
                                {
                                    this.props.message.exception?.innerException != null &&
                                    <Grid container={true} item={true}>
                                        <Grid item={true} md={12}>{NotificationRow.preRenderInnerException(this.props.message.exception?.innerException)}</Grid>
                                    </Grid>
                                }
                            </Grid>
                        </Container>
                    </Collapse>
                </TableCell >
            </TableRow >
            ]
        );

    }

    private static preRenderInnerException(exception: SerializableException): React.ReactNode {
        return (
            <Grid container={true} sx={{ paddingLeft: 1, paddingRight: 0 }} rowSpacing={0.5}>
                <Grid container={true} item={true}>
                    <Grid item={true} md={2}>
                        <Typography variant="body2" sx={{ fontWeight: "bold"}}>Type</Typography>
                    </Grid>
                    <Grid item={true} md={10}>
                        <Typography variant="body2" sx={{ fontFamily: "Monospace" }}>{exception.exceptionTypeName}</Typography>
                    </Grid>
                </Grid>
                <Grid container={true} item={true}>
                    <Grid item={true} md={2}><Typography variant="body2" sx={{ fontWeight: "bold"}}>Message</Typography></Grid>
                    <Grid item={true} md={10}> <Typography variant="body2" sx={{ fontFamily: "Monospace" }}>{exception.message}</Typography></Grid>
                </Grid>
                {exception.innerException != null &&
                    NotificationRow.preRenderInnerException(exception.innerException)
                }
            </Grid>
        );
    }
}
