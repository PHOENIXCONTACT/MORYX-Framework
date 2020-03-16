/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

export default class SerializableException
{
    ExceptionTypeName : string;
    Message : string;
    StackTrace : string;
    InnerException : SerializableException;
}
