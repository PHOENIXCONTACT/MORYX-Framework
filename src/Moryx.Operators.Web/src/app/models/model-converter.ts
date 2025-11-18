import { Entry } from "@moryx/ngx-web-framework";
import { SkillModel } from "../api/models/skill-model";
import { SkillTypeModel } from "../api/models/skill-type-model";
import { OperatorSkill } from "./operator-skill-model";
import { SkillType } from "./skill-type-model";

export function skillToOperatorSkill(skill: SkillModel): OperatorSkill{

    return <OperatorSkill>{
        id: skill.id,
        obtainedOn: new Date(skill.obtainedOn ?? ''),
        operatorId: skill.operatorIdentifier,
        isExpired: skill.isExpired,
        typeId: skill.typeId,
        expiresOn: new Date(skill.expiresOn ?? '')
    }
}


export function skillTypeModelToModel(type: SkillTypeModel): SkillType{
    return <SkillType>{
        id: type.id,
        name: type.name,
        duration: type.duration,
        acquiredCapabilities: <Entry>{},
        trainedOperators: 0 // TODO: get count from backend
    }
}

export function skillTypeToModel(type: SkillType ): SkillTypeModel{
    return <SkillTypeModel>{
        id: type.id,
        name: type.name,
        duration: type.duration,
        acquiredCapabilities: type.acquiredCapabilities,
        trainedOperators: 0 // TODO: get count from backend
    }
}