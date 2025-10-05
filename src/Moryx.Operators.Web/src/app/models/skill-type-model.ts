import { Entry } from "@moryx/ngx-web-framework";
import { TimeSpan } from "../api/models/System/time-span";

export interface SkillType{
    id: number;
    name: string;
    duration: string;
    acquiredCapabilities: Entry;
    trainedOperators: number;
}