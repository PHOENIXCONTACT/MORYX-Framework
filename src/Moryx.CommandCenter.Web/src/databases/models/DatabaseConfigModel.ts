/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Entry from "../../modules/models/Entry";

export default class DatabaseConfigModel {
    public configuratorType: string;
    public connectionString: string;
    public properties: Entry;
}
