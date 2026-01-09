import { SkillType } from "./skill-type-model";

export interface OperatorSkill {
    id: number;
    obtainedOn: Date,
    expiresOn: Date,
    typeId: number;
    operatorId: string;
    isExpired: boolean;
}