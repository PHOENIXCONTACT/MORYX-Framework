/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import MenuItemModel from "./MenuItemModel";

export interface MenuProps {
  Menu: MenuModel;

  onActiveMenuItemChanged?(menuItem: MenuItemModel): void;
}

export default interface MenuModel {
  MenuItems: MenuItemModel[];
}
