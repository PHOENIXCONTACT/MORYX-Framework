/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import List from "@mui/material/List";
import * as React from "react";
import MenuItemModel from "../../models/MenuItemModel";
import { MenuProps } from "../../models/MenuModel";
import RoutingMenuItem from "./RoutingMenuItem";

function RoutingMenu(props: MenuProps) {
  const renderMenu = (menuItems: MenuItemModel[]): React.ReactNode => {
    return <List>{
      menuItems.map((menuItem, idx) => {
        return (
          <RoutingMenuItem
            Key={idx}
            MenuItem={menuItem}
            Level={0}
            Divider={idx < menuItems.length - 1}
          />
        );
      })
    } </List>;
  };

  return <div>{renderMenu(props.Menu.MenuItems)}</div>;
}

export default RoutingMenu;
