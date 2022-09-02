/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/
import { LogLevel } from "../../models/LogLevel";

export default class AddRemoteAppenderRequest {
    public Name: string;
    public MinLevel: LogLevel;
}
