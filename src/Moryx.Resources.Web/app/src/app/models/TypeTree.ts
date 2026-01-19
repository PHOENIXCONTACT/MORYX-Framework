/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ResourceModel } from "../api/models";

/**
 * This method takes the id of the target node and returns an array of resource ids.
 * The array contains all ids of resources that lay on a path from the root to the
 * target id not including the target itself
 * @method
 */
export function getHierarchieLineFor(
  targetId: number | undefined,
  resourceTree: ResourceModel[] | undefined
): number[] {
  if (!targetId || !resourceTree) return [];
  let line: number[] = [];
  for (let r of resourceTree) {
    const possibleLine = exploreHierarchieLineFor(r, targetId);
    if (!possibleLine) continue;
    line = possibleLine;
    break;
  }
  return line;
}

/**
   * This method takes a source resource and the id of the target resource. It runs
   * a DFS on the node and its children for the resource with the matching id. The
   * method returns and empty array, if the source resource is the target. If the
   * target was found in any of the resources children it appends its own to the
   * array of ids provided by the relevant child. Otherwise, i.e. the target could
   * not be found in any of the child trees, the method returns undefined.
   * @method
   */
export function exploreHierarchieLineFor(
    source: ResourceModel,
    targetId: number
  ): number[] | undefined {
    if (source.id === targetId) return [];

    const childReferences =
      source.references?.find((ref) => ref.name == "Children")?.targets ?? [];
    if (!childReferences.length) return undefined;

    for (let c of childReferences) {
      const path = exploreHierarchieLineFor(c, targetId);
      if (!path) continue;
      path.push(source.id!);
      return path;
    }
    return undefined;
  }
