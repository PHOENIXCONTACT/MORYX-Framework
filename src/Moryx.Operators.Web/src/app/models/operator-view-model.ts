/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { AssignableOperator } from "../api/models/assignable-operator";
import { OperatorModel } from "../api/models/operator-model";
import { IOperatorAssignable } from "../api/models/i-operator-assignable";
import { OperatorSkill } from "./operator-skill-model";
import { WorkstationViewModel } from "./workstation-view-model";


export class OperatorViewModel  {

    constructor(
        private _data: AssignableOperator 
    ){
    }

    public get data(){
        return this._data;
    }
    
    public assignToStation(workstation: WorkstationViewModel ): void{
        const resource = <IOperatorAssignable>{
            id: workstation.data.id,
            name: workstation.data.name,
            requiredSkills: workstation.data
                    .requiredSkills?.map(x => <IOperatorAssignable>{
                        name: x
                    })
        }
        this._data.assignedResources?.push(resource);
    }

    public unassign(resourceId: number ){
        this._data.assignedResources = this._data.assignedResources?.filter(x => x.id !== resourceId);
    }

    public update(model: OperatorViewModel ) {

    }
}
