/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import DatabaseConfigModel from "../../models/DatabaseConfigModel";
import ScriptModel from "../../models/ScriptModel";

export default class ExecuteScriptRequest
{
    Config : DatabaseConfigModel;
    Script : ScriptModel;
}
