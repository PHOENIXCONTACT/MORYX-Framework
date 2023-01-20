/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import DatabaseUpdate from "../../models/DatabaseUpdate";

export default class ResponseModel<T> {
    public result: T;
    public errors: string[];
}
