import Entry from "./Entry";

export default class Config {
    public Module: string;
    public Root: Entry;

    public static patchConfig(config: Config): void {
        config.Root.SubEntries.forEach((entry) => {
            Config.patchParent(entry, config.Root);
            Entry.generateUniqueIdentifiers(entry);
        });
    }

    public static patchParent(entry: Entry, parentEntry: Entry): void {
        entry.Parent = parentEntry;
        entry.SubEntries.forEach((subEntry) => this.patchParent(subEntry, entry));
    }
}
