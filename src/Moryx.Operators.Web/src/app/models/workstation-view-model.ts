/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { AttendableResourceModel } from "../api/models/attendable-resource-model";
import { OperatorViewModel } from "./operator-view-model";

export class WorkstationViewModel  {

    constructor(
        private _data : AttendableResourceModel ,
    )
        {}

    public get data()
    {
        return this._data;
    }

    public assignOperatorToCurrentStation(operator: OperatorViewModel)
    {
       
    }

}
