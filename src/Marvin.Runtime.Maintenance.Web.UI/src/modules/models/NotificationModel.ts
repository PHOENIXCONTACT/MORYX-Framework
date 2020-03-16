/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Serverity } from "./Severity";
import SerializableException from "./SerializableException";

export default class NotificationModel {
    public Timestamp: Date;
    public Important: boolean;
    public Exception: SerializableException;
    public Message: string;
    public Severity: Serverity;
}
