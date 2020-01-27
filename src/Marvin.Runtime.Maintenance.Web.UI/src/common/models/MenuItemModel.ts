import * as React from "react";

export const enum IconType {
    Image,
    FontAwesome
}

export default interface MenuItemModel {
    Name: string;
    NavPath: string;
    SubMenuItems: MenuItemModel[];
    Icon?: any;
    IconType?: IconType;
    Content?: React.ReactNode;
}
