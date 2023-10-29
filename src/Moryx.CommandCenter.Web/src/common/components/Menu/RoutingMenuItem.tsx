/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiChevronDown, mdiChevronUp } from "@mdi/js";
import Icon from "@mdi/react";
import { Location, UnregisterCallback } from "history";
import * as React from "react";
import { Link, RouteComponentProps, withRouter } from "react-router-dom";
import { Col, Collapse, Container, Row } from "reactstrap";
import MenuItemModel from "../../models/MenuItemModel";

interface MenuItemProps {
    MenuItem: MenuItemModel;
    Level: number;
    onMenuItemClicked?(menuItem: MenuItemModel): void;
}

interface MenuItemState {
    IsOpened: boolean;
}

class RoutingMenuItem extends React.Component<RouteComponentProps<{}> & MenuItemProps, MenuItemState> {
    private unregisterListenerCallback: UnregisterCallback;

    constructor(props: RouteComponentProps<{}> & MenuItemProps) {
        super (props);
        this.state = { IsOpened: this.isOpened(this.props.location) };

        this.unregisterListenerCallback = this.props.history.listen(this.onRouteChanged.bind(this));
        this.onMenuItemClicked = this.onMenuItemClicked.bind(this);
    }

    public componentWillUnmount(): void {
        this.unregisterListenerCallback();
    }

    private isOpened(location: Location): boolean {
        return location.pathname.startsWith(this.props.MenuItem.NavPath);
    }

    private onRouteChanged(location: Location, action: string): void {
        this.setState({ IsOpened: this.isOpened(location) });
    }

    private handleMenuItemClick(e: React.MouseEvent<HTMLElement>): void {
        e.preventDefault();

        this.setState((prevState) => ({ IsOpened: !prevState.IsOpened }));
        this.onMenuItemClicked(this.props.MenuItem);
    }

    private onMenuItemClicked(menuItem: MenuItemModel): void {
        if (this.props.onMenuItemClicked != null) {
            this.props.onMenuItemClicked(menuItem);
        }
    }

    private renderSubMenuItems(): React.ReactNode {
        return this.props.MenuItem.SubMenuItems.map ((menuItem, idx) =>
            <RoutingMenuItem key={idx}
                             MenuItem={menuItem}
                             Level={this.props.Level + 1}
                             onMenuItemClicked={this.onMenuItemClicked}
                             match={this.props.match}
                             location={this.props.location}
                             history={this.props.history}
                             staticContext={this.props.staticContext} />);
    }

    public render(): React.ReactNode {
        const isActive = this.props.location.pathname.includes(this.props.MenuItem.NavPath);

        return (
            <div style={{paddingLeft: this.props.Level * 10 + "px", margin: "5px 0px 5px 0px"}}>
                <Container fluid={true} className="menu-item" onClick={(e: React.MouseEvent<HTMLElement>) => this.handleMenuItemClick(e)}>
                    <Row>
                        <Col md={12} style={{display: "flex"}}>
                            <Link to={this.props.MenuItem.NavPath} className={bold} style={{flex: "1"}}>
                                { this.props.MenuItem.Icon != undefined &&
                                    <Icon path={this.props.MenuItem.Icon} className="icon right-space" />
                                }
                                <span style={{wordBreak: "break-all"}}>{this.props.MenuItem.Name}</span>
                            </Link>
                            {this.props.MenuItem.Content}
                        </Col>
                    </Row>
                </Container>
            </div>
        );
    }
}

export default withRouter<RouteComponentProps<{}> & MenuItemProps, React.ComponentType<any>>(RoutingMenuItem);
