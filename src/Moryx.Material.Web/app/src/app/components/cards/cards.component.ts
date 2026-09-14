import { Component, computed, effect, inject, OnDestroy, OnInit, resource, signal } from '@angular/core';
import { CardComponent } from "../card/card.component";
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, firstValueFrom, lastValueFrom, SubscriptionLike } from 'rxjs';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDivider } from "@angular/material/divider";
import { MatButtonModule } from '@angular/material/button';
import { MatIcon } from "@angular/material/icon";
import { TranslateService } from '@ngx-translate/core';
import { MaterialFlowService } from '@app/services/material-flow.service';
import { MaterialManagementService } from '@app/api/services';
import { environment } from '../../../environments/environment';
import { fromEventStream } from '@app/utilities/server-sent-event';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { ReferenceType } from '@app/models/material-container';
import { MaterialContainerModel, OrderReferenceModel, ResourceTypeModel, StateClassificationModel } from '@app/api/models';

@Component({
  selector: 'app-cards',
  imports: [CardComponent, MatDivider, MatButtonModule, MatIcon],
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
  private translateService = inject(TranslateService);
  private filterEvents = toSignal(this.materialFlow.$filter);
  protected sectionState = signal(Object.values(StateClassificationModel).map(x => <SectionState>{ section: x, isExpanded: false }))
  protected stateMapResource = resource({
    loader: () => firstValueFrom(this.containerApi.getStates())
  })

  types = signal<ResourceTypeModel[]>([]);
  protected containers = computed(() => {
    const fetchedContainers = this.containersResource.value() ?? [];
    const filters = this.filterEvents() ?? [];
    if (filters.length > 0) {
      return fetchedContainers.filter(e => filters.some(f => this.matchOrder(e, f))) ?? [];
    }
    return fetchedContainers;
  })
  protected translationConstants = TranslationConstants;

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
    if (id <= 0) {
      return;
    }

    this.containerApi
      .deregister({ id: id })
      .subscribe({
        next: async () => {
          const translations = await this.getTranslations();
          this.snackbarService.showError(translations[TranslationConstants.CARDS.DELETED]);
          this.containersResource.reload();
        },
        error: (e: HttpErrorResponse) => {
          this.snackbarService.handleError(e);
        }
      })
  }

  async getTranslations(): Promise<{ [key: string]: string }> {
    return await lastValueFrom(this.translateService
      .get([
        TranslationConstants.CARDS.DELETED,
      ]));
  }

  matchOrder(container: MaterialContainerModel, keyword: string): boolean {
    return container.references?.some(reference => (reference as any).type?.toLowerCase().includes(ReferenceType.Order.toLowerCase()) && (reference as OrderReferenceModel).orderNumber == keyword) ?? false;
  }

  findStateName(classification: StateClassificationModel) {
    return this.stateMapResource.value()?.find(x => x.state === classification)?.stateDisplayName ?? '?';
  }
}

export interface SectionState {
  section: StateClassificationModel;
  isExpanded: boolean;
}