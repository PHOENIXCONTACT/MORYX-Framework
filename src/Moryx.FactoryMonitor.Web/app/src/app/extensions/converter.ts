/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { OrderModel } from '../api/models/order-model';
import { ActivityChangedModel } from '../api/models/activity-changed-model';
import { CellStateChangedModel } from '../api/models/cell-state-changed-model';
import { OrderChangedModel } from '../api/models/order-changed-model';
import { ResourceChangedModel } from '../api/models/resource-changed-model';
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
    if (!resourceModel.id) throw new TypeError("cannot create resource without id");
    const cell = <CellModel>{};
    cell.id = resourceModel.id;

    return this.addResourceDataToCell(cell, resourceModel)
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

  public static addResourceDataToCell(cell: CellModel, model: ResourceChangedModel): CellModel {
    if (model.cellName) {
      cell.name = model.cellName ?? ''
    }
    if (model.factoryId) {
      cell.factoryId= model.factoryId ?? ''
    }
    if (model.cellIconName) {
      cell.iconName = model.cellIconName ?? ''
    }
    if (model.cellImageURL) {
      cell.image = model.cellImageURL ?? ''
    }
    if (model.cellLocation) {
      cell.location = model.cellLocation
    }
    if (model.cellPropertySettings) {
      cell.propertySettings = model.cellPropertySettings
    }

    return cell
  }

  public static addStateDataToCell(cell: CellModel, model: CellStateChangedModel): CellModel {
    if (!cell.state) {
      cell.state = model.state
    }

    return cell
  }

  public static modelToOrder(order: OrderModel): Order {
    return <Order>{
      isToggled: true,
      operationNumber: order.operation,
      orderColor: order.color,
      orderNumber: order.order,
    };
  }
}
