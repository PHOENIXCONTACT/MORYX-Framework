import { OperatorsResourceModel } from "../api/models/Moryx/Operators/Endpoints/operators-resource-model";
import { OperatorViewModel } from "./operator-view-model";

export class WorkstationViewModel  {

    constructor(
        private _data : OperatorsResourceModel ,
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