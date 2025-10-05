/* tslint:disable */
/* eslint-disable */
import { Entry as MoryxSerializationEntry } from '../../../../models/Moryx/Serialization/entry';
import { TimeSpan as SystemTimeSpan } from '../../../../models/System/time-span';
export interface SkillTypeModel {
  capabilities?: MoryxSerializationEntry;
  duration?: SystemTimeSpan;
  id?: number;
  name?: string | null;
}
