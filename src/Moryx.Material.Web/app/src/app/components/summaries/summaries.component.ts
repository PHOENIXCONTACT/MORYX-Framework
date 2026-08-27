import { Component, inject, OnDestroy, OnInit, resource } from '@angular/core';
import { SummaryComponent } from "../summary/summary.component";
import { firstValueFrom, SubscriptionLike } from 'rxjs';
import { MaterialManagementService } from 'src/app/api/services';
import { environment } from 'src/environments/environment';
import { MaterialContainerModel, OrderReferenceModel } from 'src/app/api/models';
import { fromEventStream } from 'src/app/utilities/server-sent-event';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-summaries',
  imports: [MatCardModule],
  templateUrl: './summaries.component.html',
  styleUrl: './summaries.component.scss',
})
export class SummariesComponent implements OnInit, OnDestroy {
  private containerApi = inject(MaterialManagementService);
  private containersResource = resource({
    loader: () => firstValueFrom(this.containerApi.getContainers())
  })
  private subscriptions: SubscriptionLike[] = [];
  private stream$ = fromEventStream<MaterialContainerModel>(environment.rootUrl + MaterialManagementService.ContainerChangesPath);

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  ngOnInit(): void {
    const sub = this.stream$.subscribe(e => {
      this.containersResource.reload();
    })
    this.subscriptions.push(sub);
  }

  generateSummaries() {
    const containers = this.containersResource.value() ?? [];
    const flatList = containers.flatMap(container => container.references?.flatMap(
      r => <FlatContainerReferenceItem>{
        id: container.id,
        material: container.material,
        quantity: container.quantity,
        isOrderReference: r.fullName?.toLowerCase().includes("order"),
        orderNumber: (r as OrderReferenceModel)?.orderNumber
      }) ?? []);
    const orderByMaterialInstanceMap: Map<OrderNumber, OrderByInstanceItem[]> = new Map<string, OrderByInstanceItem[]>();
    flatList.forEach(item => {
      if (item.isOrderReference) {
        if (orderByMaterialInstanceMap.get(item.orderNumber!)) {
          this.updateOrAddOrderByInstanceRow(orderByMaterialInstanceMap, item);
        } else {
          const newOrder = <OrderByInstanceItem>{
            orderNumber: item.orderNumber!,
            material: item.material,
            materialInstanceCount: item.quantity,
          }
          orderByMaterialInstanceMap.set(item.orderNumber!, [newOrder]);
        }
      }
    });

    return orderByMaterialInstanceMap;
  }

  private updateOrAddOrderByInstanceRow(orderByMaterialInstanceMap: Map<OrderNumber, OrderByInstanceItem[]>, item: FlatContainerReferenceItem) {
    const match = orderByMaterialInstanceMap.get(item.orderNumber!);
    const matchingRow = match?.find(x => x.material === item.material);
    if (matchingRow) {
      matchingRow.materialInstanceCount += item.quantity;
    } else {
      match?.push(<OrderByInstanceItem>{ orderNumber: item.orderNumber!, material: item.material, materialInstanceCount: item.quantity });
    }
  }
}

export type OrderNumber = string;
export interface FlatContainerReferenceItem {
  id: number,
  material: string,
  quantity: number,
  isOrderReference: boolean,
  orderNumber?: string,
}
export interface OrderByInstanceItem {
  orderNumber: string,
  materialInstanceCount: number,
  material: string;
  materialTypeCount: number,
  materialType?: string
}