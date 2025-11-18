import { Entry } from "@moryx/ngx-web-framework";

export interface SkillType{
    id: number;
    name: string;
    duration: string;
    acquiredCapabilities: Entry;
    trainedOperators: number;
}