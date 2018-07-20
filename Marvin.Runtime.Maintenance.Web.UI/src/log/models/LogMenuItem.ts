import MenuItemModel from "../../common/models/MenuItemModel";
import LoggerModel from './LoggerModel';

export default interface LogMenuItem extends MenuItemModel
{
    Logger:LoggerModel;
}