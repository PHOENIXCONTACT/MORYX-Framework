/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import uuidv1 = require("uuid/v1");

export default class EntryKey {
    public Name: string;
    public Identifier: string;
    public UniqueIdentifier: string;

    constructor() {
        EntryKey.updateUniqueIdentifier(this);
    }

    public static updateUniqueIdentifier(entryKey: EntryKey): void {
        entryKey.UniqueIdentifier = uuidv1();
    }

    public static updateIdentifierToCreated(entryKey: EntryKey): void {
        entryKey.Identifier = "CREATED";
    }
}
