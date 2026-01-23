/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import  moment from 'moment';

export interface OrderModel {
  operationNumber: string;
  orderNumber: string;
  totalHours: number;
  date: Date;
}

//given a list of order and total hours of work per operators we return a 
//list of order that can be completed within the total hours of work.
export function getOrderOfTheDayBasedOnOperatorHours(
  orders: OrderModel[],
  currentDate: Date,
  operatorHours: number
): OrderModel[] {

  let hoursAvailable = operatorHours;
  let ordersOfTheDay: OrderModel[] = [];
  const filteredOrders: OrderModel[] = orders.filter(
    (x) => moment(x.date).diff(moment(currentDate), 'days') === 0
  );

  for (let order of filteredOrders) {
    if(hoursAvailable === 0) break;

    const alreadyExist = ordersOfTheDay.find(x => x.operationNumber === order.orderNumber && x.orderNumber === order.orderNumber);
    if (order.totalHours > hoursAvailable || alreadyExist) continue;

    hoursAvailable = hoursAvailable - order.totalHours;
    ordersOfTheDay.push(order);
  }

  return ordersOfTheDay;
}

export function getOrderHoursForTheDay(  orders: OrderModel[],
    currentDate: Date,
    operatorHours: number) : number{
    const results = getOrderOfTheDayBasedOnOperatorHours(orders,currentDate,operatorHours);
    return totalOrderHours(results);
}

export function totalOrderHours(orders: OrderModel[]){
    let sum = 0;
    for (const order of orders)
        sum = sum + order.totalHours;

    return sum;
}

export function getOrdersBasedOnOperatorHours(startDate: Date | undefined, 
    endDate: Date | undefined,
    orders: OrderModel[],operatorHours: number): OrderModel[] {
    if(!startDate || !endDate) return [];
    const orderList: OrderModel[] = [];
    let totalHoursLeft = operatorHours;
    const start = moment(startDate);
    const end = moment(endDate);
    for (let now = start; now.diff(end,'days') <= 0; now =  now.add(1,'days')) {
        if(totalHoursLeft <= 0) break;

        var ordersOfTheDay = getOrderOfTheDayBasedOnOperatorHours(orders,now.toDate(),operatorHours);
        if(!ordersOfTheDay.length) continue;

        orderList.push(...ordersOfTheDay);
        totalHoursLeft = totalHoursLeft - totalOrderHours(ordersOfTheDay);
    }

    return orderList;
}


