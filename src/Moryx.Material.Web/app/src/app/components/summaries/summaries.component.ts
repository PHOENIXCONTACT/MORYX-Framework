import { Component, inject, OnDestroy, OnInit, resource } from '@angular/core';
import { SummaryComponent } from "../summary/summary.component";
import { firstValueFrom, SubscriptionLike } from 'rxjs';
import { MaterialManagementService } from 'src/app/api/services';
import { environment } from 'src/environments/environment';
import { MaterialContainerModel } from 'src/app/api/models';
import { fromEventStream } from 'src/app/utilities/server-sent-event';

@Component({
  selector: 'app-summaries',
  imports: [SummaryComponent],
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

  generateSummaries(){
    const containers = this.containersResource.value() ?? [];
    //const instances = containers.map(container => <SummaryByInstanceItem>{ orderNumber:  container. })
  }
}

export interface SummaryItem {
  orderNumber: string,
}
export interface SummaryByInstanceItem extends SummaryItem {
  orderNumber: string,
  materialInstanceCount: number,
  materialName: string;
}
export interface SummaryByTypeAndInstanceItem extends SummaryByInstanceItem {
  orderNumber: string,
  materialTypeCount: number,
  typeName: string
}