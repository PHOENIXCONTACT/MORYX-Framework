/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";
import { useNavigate } from "react-router-dom";
import MenuItemModel from "../../models/MenuItemModel";
import RoutingMenuItem from "./RoutingMenuItem";
import { MenuProps } from "./TreeMenu";

function RoutingMenu(props: MenuProps) {
    const navigate = useNavigate();

    const handleMenuItemClick = (menuItem: MenuItemModel): void => {
        if (props.onActiveMenuItemChanged != null) {
            props.onActiveMenuItemChanged(menuItem);
        }
        navigate(menuItem.NavPath);
    };

    const renderMenu = (menuItems: MenuItemModel[]): React.ReactNode => {
        return menuItems.map((menuItem, idx) => {
            return (
                <RoutingMenuItem
                    key={idx}
                    MenuItem={menuItem}
                    Level={0}
                    onMenuItemClicked={(menuItem) => handleMenuItemClick(menuItem)}
                />
            );
        });
    };

    return <div>{renderMenu(props.Menu.MenuItems)}</div>;
}

export default RoutingMenu;
