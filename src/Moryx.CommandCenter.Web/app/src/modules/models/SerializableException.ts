/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

export default class SerializableException {
    public exceptionTypeName: string;
    public message: string;
    public stackTrace: string;
    public innerException: SerializableException;
}
