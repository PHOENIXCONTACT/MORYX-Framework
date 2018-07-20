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
}
