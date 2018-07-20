import Entry from "./Entry";

export default class Config {
    public Module: string;
    public Entries: Entry[];

    public static patchConfig(config: Config): void {
        config.Entries.forEach((entry) => {
            Config.patchParent(entry, null);
            Entry.generateUniqueIdentifiers(entry);
        });
    }

    public static patchParent(entry: Entry, parentEntry: Entry): void {
        entry.Parent = parentEntry;
        entry.SubEntries.forEach((subEntry) => this.patchParent(subEntry, entry));
    }
}
