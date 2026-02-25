/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ProcessHolderGroupModel } from "../api/models/process-holder-group-model";
import { ProcessHolderGroup } from "./process-holder-group-model";
import ProcessHolderNode from "./process-holder-node";

export function ConvertToProcessHolderGroup(group: ProcessHolderGroupModel) {
  return <ProcessHolderGroup>{
    ...group,
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

