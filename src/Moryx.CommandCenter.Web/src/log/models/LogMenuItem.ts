/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/
import MenuItemModel from "../../common/models/MenuItemModel";
import LoggerModel from "./LoggerModel";

export default interface LogMenuItem extends MenuItemModel {
    Logger: LoggerModel;
}
