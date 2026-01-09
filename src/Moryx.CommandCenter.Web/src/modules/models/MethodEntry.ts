/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Entry from "./Entry";

export default class MethodEntry {
    public name: string;
    public displayName: string;
    public description: string;
    public parameters: Entry;
    public isAsync: boolean;
}
