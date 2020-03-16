/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { EntryValueType } from "./EntryValueType";

export default class EntryValue {
    public Type: EntryValueType;
    public Current: string;
    public Default: string;
    public Possible: string[];
    public IsReadOnly: boolean;
}
