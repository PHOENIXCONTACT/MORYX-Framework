/*
 * Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Entry from "../../modules/models/Entry";

export class ModelConfiguratorModel {
    public name: string;
    public configuratorType: string;
    public connectionStringKeys: Record<string, string>;
    public configPrototype: Entry;
    public connectionStringPrototype: string;
}
