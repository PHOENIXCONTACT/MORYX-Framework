/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import DatabaseConfigOptionPropertyModel from "./DatabaseConfigOptionPropertyModel";


export default class DatabaseConfigOptionModel {
    name: string;
    configuratorTypename: string;
    properties: DatabaseConfigOptionPropertyModel[];
}
