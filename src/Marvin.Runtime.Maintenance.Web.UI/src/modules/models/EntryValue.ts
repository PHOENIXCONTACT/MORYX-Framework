import { EntryValueType } from "./EntryValueType";

export default class EntryValue {
    public Type: EntryValueType;
    public Current: string;
    public Default: string;
    public Possible: string[];
    public IsReadOnly: boolean;
}
