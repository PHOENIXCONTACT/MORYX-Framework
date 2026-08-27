import { Component, computed, effect, inject, OnDestroy, OnInit, resource, signal } from '@angular/core';
import { CardComponent } from "../card/card.component";
import { MaterialContainer } from 'src/app/models/material-container';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, firstValueFrom, SubscriptionLike } from 'rxjs';
import { MaterialFlowService } from 'src/app/services/material-flow.service';
import { MaterialManagementService } from 'src/app/api/services';
import { MaterialContainerModel, OrderReferenceModel, ResourceTypeModel } from 'src/app/api/models';
import { fromEventStream, ServerSentEventMessage } from 'src/app/utilities/server-sent-event';
import { environment } from 'src/environments/environment';
import { MatDialog } from '@angular/material/dialog';
import { DialogPreAdviceComponent } from 'src/app/dialogs/dialog-pre-advice/dialog-pre-advice.component';
import { Deregister$Params } from 'src/app/api/functions';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-cards',
  imports: [CardComponent],
  templateUrl: './cards.component.html',
  styleUrl: './cards.component.scss',
})
export class CardsComponent implements OnInit, OnDestroy {
  private materialFlow = inject(MaterialFlowService);
  private containerApi = inject(MaterialManagementService);
  private containersResource = resource({
    loader: () => firstValueFrom(this.containerApi.getContainers())
  })
  private stream$ = fromEventStream<MaterialContainerModel>(environment.rootUrl + MaterialManagementService.ContainerChangesPath);
  private subscriptions: SubscriptionLike[] = [];
  private snackbarService = inject(SnackbarService);
  private filterEvents = toSignal(this.materialFlow.$filter);
  
  types = signal<ResourceTypeModel[]>([]);
  protected containers = computed(() => {
    const fetchedContainers = this.containersResource.value() ?? [];
    const filters = this.filterEvents() ?? [];
    if (filters.length > 0) {
      return fetchedContainers.filter(e => filters.some(f => this.matchOrder(e, f))) ?? [];
    }
    return fetchedContainers;
  })

  constructor() {
    const typeSub = this.containerApi.getTypes().subscribe(result => this.types.set(result));
    this.subscriptions.push(typeSub);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  ngOnInit(): void {
    const sub = this.stream$.subscribe(e => {
      this.containersResource.reload();
    })
    this.subscriptions.push(sub);
  }

  findType(typeName: string) {
    return this.types().find(x => x.name === typeName);
  }

  deleteContainer(id: number) {
    this.containerApi
      .deregister({ id: id })
      .subscribe({
        next: () => {
          this.snackbarService.showError("Container deleted!");
          this.containersResource.reload();
        },
        error: (e: HttpErrorResponse) => {
          this.snackbarService.handleError(e);
        }
      })
  }

  matchOrder(container: MaterialContainerModel, keyword: string): boolean {
    return container.references?.some(reference => reference.fullName?.toLowerCase().includes("orders") && (reference as OrderReferenceModel).orderNumber == keyword) ?? false;
  }
}