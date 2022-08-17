/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

export default class SerializableException
{
    exceptionTypeName : string;
    message : string;
    stackTrace : string;
    innerException : SerializableException;
}
