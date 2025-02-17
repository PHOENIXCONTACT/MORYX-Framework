/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

export default class EntryValidation {
    public minimum: number;
    public maximum: number;
    public regex: string;
    public isRequired: boolean;
    public isPassword: boolean;
    public deniedValue: string[];
    public allowedValue: string[];
}
