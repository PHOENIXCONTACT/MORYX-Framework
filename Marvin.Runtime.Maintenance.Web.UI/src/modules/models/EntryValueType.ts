export enum EntryValueType {
    Byte,
    Boolean,
    Int16,
    UInt16,
    Int32,
    UInt32,
    Int64,
    UInt64,
    Single,
    Double,
    String,
    Enum,
    Class,
    Collection,
}

export function toString(type: EntryValueType): string {
    switch (type) {
        case EntryValueType.Byte: return "Byte";
        case EntryValueType.Boolean: return "Boolean";
        case EntryValueType.Int16: return "Int16";
        case EntryValueType.UInt16: return "UInt16";
        case EntryValueType.Int32: return "Int32";
        case EntryValueType.UInt32: return "UInt32";
        case EntryValueType.Int64: return "Int64";
        case EntryValueType.UInt64: return "UInt64";
        case EntryValueType.Single: return "Single";
        case EntryValueType.Double: return "Double";
        case EntryValueType.String: return "String";
        case EntryValueType.Enum: return "Enum";
        case EntryValueType.Class: return "Class";
        case EntryValueType.Collection: return "Collection";
        default: return "";
    }
}
