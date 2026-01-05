import { Maintenance } from './models/maintenance';
import { MaintainableResource } from './models/maintainable-resource';
import { Interval, IntervalBase, IntervalType } from './models/interval-base';
import {
  VisualInstruction,
} from './models/visual-instruction';
import { Acknowledgement } from './models/acknowledgement';
import { Entry, EntryUnitType, EntryValueType } from '@moryx/ngx-web-framework';
import { MoryxControlSystemVisualInstructionsInstructionContentType } from './api/models';

export const RESOURCES: MaintainableResource[] = [
  { id: 1, name: 'Komax' },
  { id: 2, name: 'TTC' },
  { id: 3, name: 'ClipX' },
  { id: 4, name: 'PrintX' },
];

export const MAINTENANCES: Maintenance[] = [
  {
    id: 1,
    resource: RESOURCES[0],
    maintenanceStarted: true,
    interval: <Interval>{
      interval: {
        lastMaintenanceDate: new Date(),
        elapsed: 1,
        value: 150,
        overdue: 0,
      },
      type: IntervalType.Cycle,
    },
    block: false,
    isActive: true,
    created: new Date(),
    instructions: [
      <VisualInstruction>{
        type: MoryxControlSystemVisualInstructionsInstructionContentType.Text,
        content: 'Remove the lid and empty the box',
        preview: '',
      },
    ],
    acknowledgements: [
      <Acknowledgement>{
        id: 1,
        description: 'Cleaned and changed the applicator from the contact box.',
        created: new Date(),
      },
      <Acknowledgement>{
        id: 2,
        description: 'Cleaned the applicator head.',
        created: new Date(),
      },
    ],
  },
  {
    id: 2,
    resource: RESOURCES[1],
    maintenanceStarted: false,
    interval: <Interval>{
      interval: {
        lastMaintenanceDate: new Date(),
        elapsed: 11,
        value: 10,
        overdue: 1,
      },
      type: IntervalType.Day,
    },
    block: false,
    isActive: true,
    created: new Date(),
    instructions: [
      <VisualInstruction>{
        type: MoryxControlSystemVisualInstructionsInstructionContentType.Text,
        content: 'Remove the lid and empty the box',
        preview: '',
      },
    ],
    acknowledgements: [],
  },
  {
    id: 3,
    resource: RESOURCES[2],
    maintenanceStarted: false,
    interval: <Interval>{
      interval: {
        lastMaintenanceDate: new Date(),
        elapsed: 8,
        value: 10,
        overdue: 0,
      },
      type: IntervalType.Day,
    },
    block: false,
    isActive: false,
    created: new Date(),
    instructions: [
      <VisualInstruction>{
        type: MoryxControlSystemVisualInstructionsInstructionContentType.Text,
        content: 'Remove the lid and empty the box',
        preview: '',
      },
    ],
    acknowledgements: [],
  },
];


