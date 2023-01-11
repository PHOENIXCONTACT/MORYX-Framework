/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Config from "../../models/Config";
import { ConfigUpdateMode } from "../../models/ConfigUpdateMode";

export default class SaveConfigRequest {
    public Config: Config;
    public UpdateMode: ConfigUpdateMode;
}
