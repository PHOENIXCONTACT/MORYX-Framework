import { EntryValueType } from "./EntryValueType";

export default class EntryValue
{
    Type : EntryValueType;
    Current : string;
    Default : string;
    Possible : string[];
    IsReadOnly : boolean;
}