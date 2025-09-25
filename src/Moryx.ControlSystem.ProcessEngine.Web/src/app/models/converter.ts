import { ProcessHolderGroupModel } from "../api/models/Moryx/ControlSystem/Processes/Endpoints/process-holder-group-model";
import { ProcessHolderPositionModel } from "../api/models/Moryx/ControlSystem/Processes/Endpoints/process-holder-position-model";
import { ProcessHolderGroup } from "./process-holder-group-model";
import ProcessHolderNode from "./process-holder-node";
import { ProcessHolderPosition } from "./process-holder-position-model";

export function ConvertToProcessHolderGroup(group: ProcessHolderGroupModel) {
  return <ProcessHolderGroup>{
    ...group,
  };
}

export function ConvertToProcessHolderPosition(position: ProcessHolderPositionModel) {
  return <ProcessHolderPosition>{
    ...position,
  };
}

export function ConvertToNode(group: ProcessHolderGroupModel) {
  const node: ProcessHolderNode = {
    name: group.name ?? '?',
    data: ConvertToProcessHolderGroup(group),
    children: group.positions?.map(
      (x) => <ProcessHolderNode>{ name: x.name, data: x }
    ),
  };
  return node;
}
