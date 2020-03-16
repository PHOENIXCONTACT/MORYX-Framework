/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import DatabaseConfigModel from "../../models/DatabaseConfigModel";
import SetupModel from "../../models/SetupModel";

export default interface ExecuteSetupRequest
{
    Config : DatabaseConfigModel;
    Setup : SetupModel;
}
