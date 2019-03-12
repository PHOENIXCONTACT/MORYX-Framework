import Config from "./Config";
import EntryKey from "./EntryKey";
import EntryValidation from "./EntryValidation";
import EntryValue from "./EntryValue";

export default class Entry {
    public Key: EntryKey;
    public Value: EntryValue;
    public SubEntries: Entry[];
    public Prototypes: Entry[];
    public Description: string;
    public Validation: EntryValidation;
    public Parent: Entry;

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
        EntryKey.updateUniqueIdentifier(entry.Key);
        entry.SubEntries.forEach((subEntry: Entry) => {
            Entry.generateUniqueIdentifiers(subEntry);
        });
    }

    public static cloneFromPrototype(prototype: Entry, parent: Entry): Entry {
        const entryClone = JSON.parse(JSON.stringify(prototype));

        Config.patchParent(entryClone, parent);

        EntryKey.updateIdentifierToCreated(entryClone.Key);
        Entry.generateUniqueIdentifiers(entryClone);
        return entryClone;
    }
}
