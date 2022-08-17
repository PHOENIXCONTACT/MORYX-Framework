/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Serverity } from "./Severity";
import SerializableException from "./SerializableException";

export default class NotificationModel {
    public timestamp: Date;
    public important: boolean;
    public exception: SerializableException;
    public message: string;
    public severity: Serverity;
}
