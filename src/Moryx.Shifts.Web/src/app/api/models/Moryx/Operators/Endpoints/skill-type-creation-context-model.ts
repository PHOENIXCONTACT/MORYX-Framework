/* tslint:disable */
/* eslint-disable */
import { Entry as MoryxSerializationEntry } from '../../../../models/Moryx/Serialization/entry';
import { TimeSpan as SystemTimeSpan } from '../../../../models/System/time-span';
export interface SkillTypeCreationContextModel {
  capabilities?: MoryxSerializationEntry;
  duration?: SystemTimeSpan;
  name?: string | null;
}
