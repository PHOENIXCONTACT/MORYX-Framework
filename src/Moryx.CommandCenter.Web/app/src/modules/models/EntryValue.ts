/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import EntryPossible from "./EntryPossible";
import { EntryUnitType } from "./EntryUnitType";
import { EntryValueType } from "./EntryValueType";

export default class EntryValue {
    public type: EntryValueType;
    public unitType: EntryUnitType;
    public current: string;
    public default: string;
    public possible: EntryPossible[];
    public isReadOnly: boolean;
}
