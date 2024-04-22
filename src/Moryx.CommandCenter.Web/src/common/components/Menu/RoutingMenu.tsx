/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import List from "@mui/material/List";
import * as React from "react";
import { useNavigate } from "react-router-dom";
import MenuItemModel from "../../models/MenuItemModel";
import { MenuProps } from "../../models/MenuModel";
import RoutingMenuItem from "./RoutingMenuItem";

function RoutingMenu(props: MenuProps) {
    const navigate = useNavigate();

    const handleMenuItemClick = (menuItem: MenuItemModel): void => {
        if (props.onActiveMenuItemChanged != null) {
            props.onActiveMenuItemChanged(menuItem);
        }
        navigate(menuItem.NavPath);
    };

    const renderMenu = (menuItems: MenuItemModel[]): React.ReactNode => {
        return <List>{
            menuItems.map((menuItem, idx) => {
                return (
                    <RoutingMenuItem
                        Key={idx}
                        MenuItem={menuItem}
                        Level={0}
                        Divider={idx < menuItems.length - 1}
                        onMenuItemClicked={(menuItem) => handleMenuItemClick(menuItem)}
                    />
                );
            })
        } </List>;
    };

    return <div>{renderMenu(props.Menu.MenuItems)}</div>;
}

export default RoutingMenu;
