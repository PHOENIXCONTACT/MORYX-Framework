/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import DataModel from "./DataModel";
import { ModelConfiguratorModel } from "./ModelConfiguratorModel";

export default class DatabaseAndConfigurators {
  public database: DataModel;
  public configurators: ModelConfiguratorModel[];
}
