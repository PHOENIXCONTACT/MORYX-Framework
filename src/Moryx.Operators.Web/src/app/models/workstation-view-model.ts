import { AttendableResourceModel } from "../api/models/Moryx/Operators/Endpoints/attendable-resource-model";
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