/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { OrderModel } from '@api/models/order-model';
import { ActivityChangedModel } from '@api/models/activity-changed-model';
import { CellStateChangedModel } from '@api/models/cell-state-changed-model';
import { OrderChangedModel } from '@api/models/order-changed-model';
import { ResourceChangedModel } from '@api/models/resource-changed-model';
import CellModel from '../models/cellModel';
import Order from '../models/order';

export class Converter {

  public static activityChangedModelToCell(activityModel: ActivityChangedModel): CellModel {
    const cell = <CellModel>{};
    cell.id = activityModel.resourceId ?? 0;

    return this.addActivityChangedModelToCell(cell, activityModel)
  }

  public static addActivityChangedModelToCell(cell: CellModel, activityModel: ActivityChangedModel): CellModel {
    cell.classification = activityModel.classification;
    cell.operationNumber = activityModel.orderReferenceModel?.operation ?? '';
    cell.orderNumber = activityModel.orderReferenceModel?.order ?? '';

    return cell
  }

  public static cellStateChangedModelToCell(cellModel: CellStateChangedModel): CellModel {
    const cell = <CellModel>{};
    cell.id = cellModel.id ?? 0;
    cell.state = cellModel.state;

    return cell
  }

  public static resourceChangedModelToCell(resourceModel: ResourceChangedModel): CellModel {
    if (!resourceModel.id) {
      throw new TypeError("Cannot create resource without id");
    }
    const cell = <CellModel>{};
    cell.id = resourceModel.id;

    if (resourceModel.cellName) {
      cell.name = resourceModel.cellName
    }
    if (resourceModel.factoryId) {
      cell.factoryId= resourceModel.factoryId
    }
    if (resourceModel.cellIconName) {
      cell.iconName = resourceModel.cellIconName
    }
    if (resourceModel.cellImageURL) {
      cell.image = resourceModel.cellImageURL
    }
    if (resourceModel.cellLocation) {
      cell.location = resourceModel.cellLocation
    }
    if (resourceModel.cellPropertySettings) {
      cell.propertySettings = resourceModel.cellPropertySettings
    }

    return cell;
  }

  public static orderModelToOrder(orderModel: OrderModel): Order {
    const order = <Order>{};

    order.isToggled = true;
    order.orderNumber = orderModel.order ?? '';
    order.operationNumber = orderModel.operation ?? '';
    order.orderColor = orderModel.color ?? '';
    order.classification = orderModel.state;

    return order
  }

  public static orderChangedModelToOrder(orderModel: OrderChangedModel): Order {
    const order = <Order>{};

    order.isToggled = true
    order.orderNumber = orderModel.order ?? ''
    order.operationNumber = orderModel.operation ?? ''
    order.classification = orderModel.state

    return order
  }

  public static addStateDataToCell(cell: CellModel, model: CellStateChangedModel): CellModel {
    if (model.state) {
      cell.state = model.state
    }

    return cell
  }
}
