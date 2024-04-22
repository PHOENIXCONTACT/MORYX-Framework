/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import * as React from "react";

export const enum IconType {
    Image = "Image",
    Icon = "Icon",
}

export default interface MenuItemModel {
    Secondary?: string;
    Name: string;
    NavPath: string;
    SubMenuItems: MenuItemModel[];
    Icon?: any;
    IconType?: IconType;
    Content?: React.ReactNode;
}
