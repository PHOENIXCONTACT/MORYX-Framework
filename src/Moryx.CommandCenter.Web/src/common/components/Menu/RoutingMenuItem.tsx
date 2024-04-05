/*
* Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
* Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { Link, Location, useLocation, useNavigate } from "react-router-dom";
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

function RoutingMenuItem(props: MenuItemProps) {
    const location = useLocation();
    const navigate = useNavigate();

    const isOpened = (location: Location): boolean => {
        return location.pathname.startsWith(props.MenuItem.NavPath);
    };

    const [IsOpened, setIsOpened] = React.useState<boolean>(isOpened(location));

    React.useEffect(() => {
        setIsOpened(isOpened(location));
    }, [navigate]);

    const handleMenuItemClick = (e: React.MouseEvent<HTMLElement>): void => {
        e.preventDefault();
        setIsOpened((prevState) => !prevState);
        onMenuItemClicked(props.MenuItem);
    };

    const onMenuItemClicked = (menuItem: MenuItemModel): void => {
        if (props.onMenuItemClicked != null) {
            props.onMenuItemClicked(menuItem);
        }
    };

    const isActive = (location: Location): boolean => {
        // Path has to be equal to be 'active' or must be a sub path (following
        // after a `/`). Otherwise, with similar entries, multiple list items
        // could be highlighted. E.g.: 'Orders' and 'OrdersSimulator' would both
        // match the condition of `OrdersSimulator.startsWith(Orders)`.
        return location.pathname === props.MenuItem.NavPath
           || (location.pathname.startsWith(props.MenuItem.NavPath)
                && location.pathname.replace(props.MenuItem.NavPath, "")[0] === "/");
    };

    const isLocationActive = isActive(location);

    return (
        <ListGroupItem active={isLocationActive} className="menu-item" onClick={(e: React.MouseEvent<HTMLElement>) => handleMenuItemClick(e)}>
            <Link to={props.MenuItem.NavPath}>
                {props.MenuItem.Name}
            </Link>
            {props.MenuItem.Content}
        </ListGroupItem>
    );
}

export default RoutingMenuItem;
