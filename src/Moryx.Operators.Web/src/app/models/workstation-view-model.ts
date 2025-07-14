import { ResourceModel } from "../api/models/Moryx/Operators/Endpoints/resource-model";
import { OperatorViewModel } from "./operator-view-model";

export class WorkstationViewModel  {

    constructor(
        private _data : ResourceModel ,
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