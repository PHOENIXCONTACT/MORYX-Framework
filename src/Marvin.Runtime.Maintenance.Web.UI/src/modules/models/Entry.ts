/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import uuidv1 = require("uuid/v1");
import Config from "./Config";
import EntryValidation from "./EntryValidation";
import EntryValue from "./EntryValue";

export default class Entry {
    public DisplayName: string;
    public Identifier: string;
    public UniqueIdentifier: string;
    public Value: EntryValue;
    public SubEntries: Entry[];
    public Prototypes: Entry[];
    public Description: string;
    public Validation: EntryValidation;
    public Parent: Entry;

    constructor() {
        this.UniqueIdentifier = uuidv1();
        this.Value = new EntryValue();
        this.SubEntries = [];
        this.Prototypes = [];
        this.Validation = new EntryValidation();
    }

    public static entryChain(entry: Entry): Entry[] {
        const entryChain: Entry[] = [entry];
        let currentEntry = entry;
        while (currentEntry != null) {
            if (currentEntry.Parent != null) {
                entryChain.push(currentEntry.Parent);
            }

            currentEntry = currentEntry.Parent;
        }

        entryChain.reverse();
        return entryChain;
    }

    public static generateUniqueIdentifiers(entry: Entry): void {
        entry.UniqueIdentifier = uuidv1();
        entry.SubEntries.forEach((subEntry: Entry) => {
            Entry.generateUniqueIdentifiers(subEntry);
        });
    }

    public static cloneFromPrototype(prototype: Entry, parent: Entry): Entry {
        const entryClone = JSON.parse(JSON.stringify(prototype));

        Config.patchParent(entryClone, parent);

        entryClone.Identifier = "CREATED";
        Entry.generateUniqueIdentifiers(entryClone);
        return entryClone;
    }
}
