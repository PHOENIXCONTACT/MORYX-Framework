/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Entry from "./Entry";

export default class Config {
    public module: string;
    public root: Entry;

    public static patchConfig(config: Config): void {
        config.root.subEntries.forEach((entry) => {
            Config.patchParent(entry, config.root);
            Entry.generateUniqueIdentifiers(entry);
        });
    }

    public static patchParent(entry: Entry, parentEntry: Entry): void {
        entry.parent = parentEntry;
        entry.subEntries.forEach((subEntry) => this.patchParent(subEntry, entry));
    }
}
