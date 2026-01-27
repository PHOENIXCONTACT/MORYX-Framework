/*
* Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
* Licensed under the Apache License, Version 2.0
*/

import ListItem from "@mui/material/ListItem";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemText from "@mui/material/ListItemText";
import * as React from "react";
import { Location, NavLink, useLocation } from "react-router";
import MenuItemModel from "../../models/MenuItemModel";

interface MenuItemProps {
  Key: number;
  MenuItem: MenuItemModel;
  Level: number;
  Divider: boolean;
}

function RoutingMenuItem(props: MenuItemProps) {
  const location = useLocation();

  const isActive = (location: Location): boolean => {
    // Path has to be equal to be 'active' or must be a sub path (following
    // After a `/`). Otherwise, with similar entries, multiple list items
    // Could be highlighted. E.g.: 'Orders' and 'OrdersSimulator' would both
    // Match the condition of `OrdersSimulator.startsWith(Orders)`.
    return location.pathname === props.MenuItem.NavPath
      || (location.pathname.startsWith(props.MenuItem.NavPath)
        && location.pathname.replace(props.MenuItem.NavPath, "")[0] === "/");
  };

  const isLocationActive = isActive(location);

  return (
    <ListItem
      key={props.Key}
      secondaryAction={props.MenuItem.Content}
      disablePadding={true}
      component={NavLink} to={props.MenuItem.NavPath} sx={{color: "black"}}
    >
      <ListItemButton
        selected={isLocationActive}

        divider={props.Divider}
      >
        <ListItemText
          primary={props.MenuItem.Name}
          secondary={props.MenuItem.SecondaryName}
          secondaryTypographyProps={{fontSize: "x-small"}}>
          {props.MenuItem.Content}
        </ListItemText>
      </ListItemButton>
    </ListItem>
  );
}

export default RoutingMenuItem;
