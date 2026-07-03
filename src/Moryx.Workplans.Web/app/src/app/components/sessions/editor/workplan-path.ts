/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { NodeConnectionPoint, NodeConnector } from "@api/models";
import { WorkplanNodeModel } from "@api/models/workplan-node-model";
import { Position } from "./position";

export class Segment {
  constructor(start: Position, end: Position) {
    if (start.top <= end.top) {
      this.top = start.top;
    } else {
      this.top = end.top;
    }

    if (start.left <= end.left) {
      this.left = start.left;
    } else {
      this.left = end.left;
    }

    this.width = start.left == end.left ? 2 : Math.abs(end.left - start.left);
    this.height = start.top == end.top ? 2 : Math.abs(end.top - start.top);
  }

  top: number;
  left: number;
  width: number;
  height: number;
}

export class NodeConnectionPath {
  startNode!: WorkplanNodeModel;
  startInput!: NodeConnectionPoint;
  endNode!: WorkplanNodeModel;
  endInput!: NodeConnectionPoint;
  segments: Segment[] = [];

  static findPath(node: WorkplanNodeModel, output: NodeConnectionPoint, connection: NodeConnector, scale: number, stepSize: number): NodeConnectionPath {
    const factor = (output.index ?? 1) + 1;
    const pathCurveWidth = 5 * stepSize;

    const startId = 'out_' + node.id + '-' + output.index;
    const endId = 'in_' + connection.nodeId + '-' + connection.index;
    const path = new NodeConnectionPath();
    path.startNode = node;
    path.startInput = output;

    // Get start (output-span) and end (input-span)
    const start = document.getElementById(startId);
    const end = document.getElementById(endId);

    if (!start || !start.parentElement || !start.parentElement.parentElement ||
      !end || !end.parentElement || !end.parentElement.parentElement) {
      return path;
    }

    // Calculate absolute start position from DOM element dimensions
    const startPosition = this.calculatePosition(start, scale);

    // Calculate absolute end position from DOM element dimensions
    const endPosition = this.calculatePosition(end, scale);

    // Determine intermediate steps
    if (startPosition.top < endPosition.top) {
      const firstStop = new Position(startPosition.left, startPosition.top + stepSize * factor);
      const secondStop = new Position(endPosition.left, firstStop.top);

      // Construct path from start to finish
      path.segments.push(new Segment(startPosition, firstStop));
      path.segments.push(new Segment(firstStop, secondStop));
      path.segments.push(new Segment(secondStop, endPosition));
    } else {
      const firstStop = new Position(startPosition.left, startPosition.top + stepSize * factor);
      const secondStop = new Position(endPosition.left + pathCurveWidth, firstStop.top);
      const thirdStop = new Position(secondStop.left, endPosition.top - stepSize);
      const fourthStop = new Position(endPosition.left, thirdStop.top);

      // Construct path from start to finish
      path.segments.push(new Segment(startPosition, firstStop));
      path.segments.push(new Segment(firstStop, secondStop));
      path.segments.push(new Segment(secondStop, thirdStop));
      path.segments.push(new Segment(thirdStop, fourthStop));
      path.segments.push(new Segment(fourthStop, endPosition));
    }

    return path;
  }

  static calculatePosition(connector: HTMLElement, scale: number): Position {
    if (!connector.parentElement || !connector.parentElement.parentElement) {
      return new Position(0, 0);
    }

    const connectorRect = connector.getBoundingClientRect()
    const nodeRect = connector.parentElement.parentElement.getBoundingClientRect();
    const nodePosition = window.getComputedStyle(connector.parentElement.parentElement)

    const leftOffset = (connectorRect.left - nodeRect.left) + connectorRect.width / 2;
    const topOffset = (connectorRect.top - nodeRect.top) + connectorRect.height / 2;
    const position = new Position(parseInt(nodePosition.left) + leftOffset / scale, parseInt(nodePosition.top) + topOffset / scale);
    return position;
  }
}
