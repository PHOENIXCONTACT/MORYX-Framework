/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import uuidv1 = require("uuid/v1");
import Config from "./Config";
import EntryValidation from "./EntryValidation";
import EntryValue from "./EntryValue";
import { EntryValueType } from "./EntryValueType";

export default class Entry {
    public displayName: string;
    public identifier: string;
    public uniqueIdentifier: string;
    public value: EntryValue;
    public subEntries: Entry[];
    public prototypes: Entry[];
    public description: string;
    public validation: EntryValidation;
    public parent: Entry;

    constructor() {
        this.uniqueIdentifier = uuidv1();
        this.value = new EntryValue();
        this.subEntries = [];
        this.prototypes = [];
        this.validation = new EntryValidation();
    }

    public static isClassOrCollection(entry: Entry): boolean {
        return entry.value.type === EntryValueType.Class || entry.value.type === EntryValueType.Collection;
    }

    public static entryChain(entry: Entry): Entry[] {
        const entryChain: Entry[] = [entry];
        let currentEntry = entry;
        while (currentEntry != null) {
            if (currentEntry.parent != null) {
                entryChain.push(currentEntry.parent);
            }

            currentEntry = currentEntry.parent;
        }

        entryChain.reverse();
        return entryChain;
    }

    public static generateUniqueIdentifiers(entry: Entry): void {
        entry.uniqueIdentifier = uuidv1();
        entry.subEntries.forEach((subEntry: Entry) => {
            Entry.generateUniqueIdentifiers(subEntry);
        });
    }

    /**
     * @deprecated It should be replaced by entryFromPrototype after the major release
     */
    public static cloneFromPrototype(prototype: Entry, parent: Entry): Entry {
        return Entry.entryFromPrototype(prototype, parent);
    }

    public static entryFromPrototype(prototype: Entry, parent: Entry): Entry {
        const entryPrototype = JSON.parse(JSON.stringify(prototype));

        Config.patchParent(entryPrototype, parent);

        entryPrototype.Identifier = "CREATED";
        Entry.generateUniqueIdentifiers(entryPrototype);
        return entryPrototype;
    }
}
