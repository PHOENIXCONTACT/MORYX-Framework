import { Component, effect, inject, OnDestroy, OnInit, resource, signal } from '@angular/core';
import { CardComponent } from "../card/card.component";
import { MaterialContainer } from 'src/app/models/material-container';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, SubscriptionLike } from 'rxjs';
import { MaterialFlowService } from 'src/app/services/material-flow.service';
import { MaterialManagementService } from 'src/app/api/services';
import { MaterialContainerModel, ResourceTypeModel } from 'src/app/api/models';
import { fromEventStream, ServerSentEventMessage } from 'src/app/utilities/server-sent-event';
import { environment } from 'src/environments/environment';
import { MatDialog } from '@angular/material/dialog';
import { DialogPreAdviceComponent } from 'src/app/dialogs/dialog-pre-advice/dialog-pre-advice.component';

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
  private containersSource = toSignal(this.containerApi.getAll());
  containers = signal<MaterialContainerModel[]>([]);
  private stream$ = fromEventStream<MaterialContainerModel>(environment.rootUrl + MaterialManagementService.ContainerChangesPath);
  private subscriptions: SubscriptionLike[] = [];
  types = signal<ResourceTypeModel[]>([]);

  constructor() {
    const typeSub = this.containerApi.getTypes().subscribe(result => this.types.set(result));
    this.subscriptions.push(typeSub);

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
      let match = this.containers().find(x => x.id === e.data.id);
      if (match) {
        this.updateContainer(e.data);
      } else {
        this.containers.set([...this.containers(), e.data])
      }
    })
    this.subscriptions.push(sub);
  }

  updateContainer(e: MaterialContainerModel) {
    const updatedList = [...this.containers()];
    updatedList[updatedList.findIndex(i => i.id === e.id)] = e;
    this.containers.set(updatedList);
  }

  findType(typeName: string) {
    return this.types().find(x => x.name === typeName);
  }
}

