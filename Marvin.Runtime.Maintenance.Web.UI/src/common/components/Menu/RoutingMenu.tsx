import * as React from "react";
import { Link, RouteComponentProps, withRouter } from "react-router-dom";
import { Collapse } from "reactstrap";
import MenuItemModel from "../../models/MenuItemModel";
import MenuModel from "../../models/MenuModel";
import RoutingMenuItem from "./RoutingMenuItem";
import { MenuProps } from "./TreeMenu";

class RoutingMenu extends React.Component<RouteComponentProps<{}> & MenuProps, {}> {

    constructor(props: RouteComponentProps<{}> & MenuProps) {
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
                <RoutingMenuItem key={idx} MenuItem={menuItem} Level={0} onMenuItemClicked={this.handleMenuItemClick.bind(this)} />
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

export default withRouter<RouteComponentProps<{}> & MenuProps>(RoutingMenu);
