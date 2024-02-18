/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Location, UnregisterCallback } from "history";
import * as React from "react";
import { Link, RouteComponentProps, withRouter } from "react-router-dom";
import { ListGroupItem } from "reactstrap";
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
        super(props);
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

    public render(): React.ReactNode {
        const isActive = this.props.location.pathname.includes(this.props.MenuItem.NavPath);

        return (
            <ListGroupItem active={isActive} className="menu-item" onClick={(e: React.MouseEvent<HTMLElement>) => this.handleMenuItemClick(e)}>
                <Link to={this.props.MenuItem.NavPath}>
                    {this.props.MenuItem.Name}
                </Link>
                {this.props.MenuItem.Content}
            </ListGroupItem >
        );
    }
}

export default withRouter<RouteComponentProps<{}> & MenuItemProps, React.ComponentType<any>>(RoutingMenuItem);
