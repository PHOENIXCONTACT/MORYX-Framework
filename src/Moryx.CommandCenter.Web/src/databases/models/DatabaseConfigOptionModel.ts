/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import DatabaseConfigOptionPropertyModel from "./DatabaseConfigOptionPropertyModel";

export default class DatabaseConfigOptionModel {
    public name: string;
    public configuratorTypename: string;
    public properties: DatabaseConfigOptionPropertyModel[];
}
