import * as React from "react";
import { Collapse } from "reactstrap";
import MenuItemModel from "../../models/MenuItemModel";
import MenuModel from "../../models/MenuModel";
import TreeMenuItem from "./TreeMenuItem";

export interface MenuProps {
    Menu: MenuModel;
    onActiveMenuItemChanged?(menuItem: MenuItemModel): void;
}

export default class TreeMenu extends React.Component<MenuProps, {}> {

    constructor(props: MenuProps) {
        super (props);
        this.state = {};
    }

    protected handleMenuItemClick(menuItem: MenuItemModel): void {
        if (this.props.onActiveMenuItemChanged != null) {
            this.props.onActiveMenuItemChanged(menuItem);
        }
    }

    protected renderMenu(menuItems: MenuItemModel[]): React.ReactNode {
        return menuItems.map ((menuItem, idx) => {
            return (
                <TreeMenuItem key={idx} MenuItem={menuItem} Level={0} onMenuItemClicked={this.handleMenuItemClick.bind(this)} />
            );
        });
    }

    public render(): React.ReactNode {
        return (
            <div>
                {this.renderMenu(this.props.Menu.MenuItems)}
            </div>
        );
    }
}
