import { MoryxOperatorsIOperatorAssignable } from "../api/models";
import { AssignableOperator } from "../api/models/Moryx/Operators/assignable-operator";
import { OperatorModel } from "../api/models/Moryx/Operators/Endpoints/operator-model";
import { IOperatorAssignable } from "../api/models/Moryx/Operators/i-operator-assignable";
import { IOperatorSkillCapability } from "../api/models/Moryx/Operators/Skills/i-operator-skill-capability";
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
                    .requiredSkills?.map(x => <IOperatorSkillCapability>{
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