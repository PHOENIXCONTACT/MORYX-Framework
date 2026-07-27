import { Component, effect, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { CardComponent } from "../card/card.component";
import { MaterialContainer } from 'src/app/models/material-container';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, SubscriptionLike } from 'rxjs';
import { MaterialFlowService } from 'src/app/services/material-flow.service';
import { MaterialManagementService } from 'src/app/api/services';
import { MaterialContainerModel } from 'src/app/api/models';
import { fromEventStream, ServerSentEventMessage } from 'src/app/utilities/server-sent-event';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-cards',
  imports: [CardComponent],
  templateUrl: './cards.component.html',
  styleUrl: './cards.component.scss',
})
export class CardsComponent implements OnInit, OnDestroy {
  private materialFlow = inject(MaterialFlowService);
  filterEvents = toSignal(this.materialFlow.$filter);
  private containerApi = inject(MaterialManagementService);
  private containersSource = toSignal(this.containerApi.getAll_2());
  containers = signal<MaterialContainerModel[]>([]);
  private  stream$ = fromEventStream<MaterialContainerModel>(environment.rootUrl + MaterialManagementService.ContainerChangesPath);
  private subscriptions: SubscriptionLike[] = [];

  constructor() {
    effect(() => {
      const filters = this.filterEvents() ?? [];
      if (filters.length > 0) {
        this.containers.set(this.containersSource()?.filter(e => filters.some(f => e.name?.includes(f) || e.material?.includes(f))) ?? []);
      } else {
        this.containers.set(this.containersSource() ?? []);
      }
    })
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  ngOnInit(): void {
    const sub = this.stream$.subscribe(e => {
      this.updateContainer(e.data);
    })
    this.subscriptions.push(sub);
  }
  
  updateContainer(e: MaterialContainerModel) {
    this.containers.update(items => {
      const match = items.find(x => x.id === e.id);
      if(match){
        Object.assign(match, e);
      }
      return items;
    })
  }
}

