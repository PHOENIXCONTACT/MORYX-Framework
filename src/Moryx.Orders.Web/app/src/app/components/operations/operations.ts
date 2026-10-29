/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  DestroyRef,
  inject,
  OnInit,
  signal,
  viewChild,
  computed
} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatDrawer, MatSidenavModule } from '@angular/material/sidenav';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { SearchBarService, SearchRequest, SearchSuggestion, SnackbarService } from '@moryx/ngx-web-framework/services';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';
import { TranslationConstants } from '@app/translation-constants';
import { OperationService } from '@app/services/operation.service';
import { OrderManagementService } from '@api/services/order-management.service';
import { BeginDialog, BeginDialogData } from '@app/dialogs/begin-dialog/begin-dialog';
import { CreateDialog } from '@app/dialogs/create-dialog/create-dialog';
import { ReportDialog, ReportDialogData } from '@app/dialogs/report-dialog/report-dialog';
import { InterruptDialog } from '@app/dialogs/interrupt-dialog/interrupt-dialog';
import { InterruptDialogData } from '@app/dialogs/interrupt-dialog/interrupt-dialog-data';
import { OperationViewModel } from '@app/models/operation-view-model';
import { LogLevel, OperationModel, OperationStateClassification, ReportContext, ReportModel } from '@api/models';
import { MediaMatcher } from '@angular/cdk/layout';
import { DrawerContent } from './drawer-content';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { LogMessageList } from './log-message-list/log-message-list';
import { PartList } from './part-list/part-list';
import { OperationSource } from './operation-source/operation-source';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { MatBadgeModule } from '@angular/material/badge';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatToolbarModule } from '@angular/material/toolbar';
import { FilterService } from '@app/services/filter.service';
import { OperationsFilter } from './operations-filter/operations-filter';
import { MultiProgressBar } from '@app/multi-progress-bar/multi-progress-bar';

@Component({
  selector: 'app-operations',
  templateUrl: './operations.html',
  styleUrls: ['./operations.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    CommonModule,
    TranslatePipe,
    MatIconModule,
    MatDrawer,
    MatSidenavModule,
    LogMessageList,
    OperationsFilter,
    PartList,
    OperationSource,
    MatExpansionModule,
    MatButtonModule,
    MatBadgeModule,
    EmptyState,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatSidenavModule,
    MatToolbarModule,
    MultiProgressBar
  ],
  host: {
    '(window:beforeunload)': 'disconnectEvents()'
  }
})
export class Operations implements OnInit {
  private orderManagementService = inject(OrderManagementService);
  private dialog = inject(MatDialog);
  private router = inject(Router);
  private location = inject(Location);
  private activatedRoute = inject(ActivatedRoute);
  private searchBarService = inject(SearchBarService);
  private translateService = inject(TranslateService);
  private snackbarService = inject(SnackbarService);
  private operationService = inject(OperationService);
  private changeDetectorRef = inject(ChangeDetectorRef);
  private mediaMatcher = inject(MediaMatcher);
  private filterService = inject(FilterService);
  private destroyRef = inject(DestroyRef);

  protected operations = signal<OperationViewModel[]>([]);
  protected filteredOperations = computed(() =>
    this.filteringOperations(this.operations())
  );

  protected DrawerContent = DrawerContent;
  protected drawerContent = signal<DrawerContent>(DrawerContent.None);
  protected selectedOperation = signal<OperationModel | undefined>(undefined);
  protected TranslationConstants = TranslationConstants;
  protected OperationStateClassification = OperationStateClassification;
  protected isLoading = signal<boolean>(true);
  protected mobileQuery: MediaQueryList;
  protected readonly drawer = viewChild.required<MatDrawer>('drawer');
  protected hideCompleted = this.filterService.hideCompleted;
  protected selectedOperationId = signal<string | undefined>(undefined);

  constructor() {
    this.mobileQuery = this.mediaMatcher.matchMedia('(max-width: 1279px)');
    this._mobileQueryListener = () => this.changeDetectorRef.detectChanges();
    this.mobileQuery.addEventListener('change', this._mobileQueryListener);
    this.destroyRef.onDestroy(() => this.disconnectEvents());
  }

  private readonly _mobileQueryListener: () => void;

  ngOnInit() {
    // Get all the operations
    this.orderManagementService.getOperations().then((operationResponse: OperationModel[]) => {
      this.operations.set(operationResponse
        .map(model => {
          const viewModel = new OperationViewModel(model);
          this.subscribeForMessagesCount(viewModel);
          return viewModel;
        }))
      this.isLoading.set(false);
      this.selectOperationFromQueryParam();
    }).catch(async (err: HttpErrorResponse) => {
      await this.snackbarService.handleError(err);
      this.isLoading.set(false);
    });

    // Searchbar
    this.searchBarService.subscribe({
      next: (request: SearchRequest) => {
        this.onSearch(request);
      }
    });

    // Register events
    this.operationService.connect((model) => this.onOperationUpdated(model));

  }

  private onOperationUpdated(updatedOperation: OperationModel) {
    if (!updatedOperation) {
      return;
    }

    this.operations.update(operations => {
      // Create a new array with replaced/added item to trigger computed() re-evaluation.
      // errorMessagesCount is preserved because it is fetched separately via getLogs.

      const index = operations.findIndex(o => o.model.identifier == updatedOperation.identifier);
      if (index >= 0) {
        const updated = new OperationViewModel(updatedOperation);
        updated.errorMessagesCount = operations[index].errorMessagesCount;

        // replace on previous index
        const copy = [...operations];
        copy[index] = updated;

        return copy;
      }
      return [...operations, new OperationViewModel(updatedOperation)];
    });
  }

  private selectOperationFromQueryParam() {
    const id = this.activatedRoute.snapshot.queryParamMap.get('selected');
    if (id) {
      this.selectedOperationId.set(id);
    }
  }

  private updateQueryParam(identifier: string | undefined) {
    void this.router.navigate([], {
      queryParams: identifier ? { selected: identifier } : {},
      replaceUrl: true
    });
  }

  private onSearch(request: SearchRequest) {
    if (request.submitted) {
      const matches = this.operations().filter(o => this.isMatch(o, request.term));
      if (matches.length === 1) {
        const id = matches[0].model.identifier!;
        this.selectedOperationId.set(id);
        this.updateQueryParam(id);
      }
      this.searchBarService.subscribe({
        next: (newRequest: SearchRequest) => {
          this.onSearch(newRequest);
        }
      });
    } else {
      const suggestions: SearchSuggestion[] = this.operations()
        .filter(o => this.isMatch(o, request.term))
        .map(o => ({
          text: `${o.model.order ?? ''}-${o.model.number ?? ''}`,
          url: this.location.prepareExternalUrl(this.router.serializeUrl(
            this.router.createUrlTree(['operations'], {
              queryParams: {
                selected: o.model.identifier
              }
            })
          ))
        }));
      this.searchBarService.provideSuggestions(suggestions);
    }
  }

  protected filteringOperations(operations: OperationViewModel[]): OperationViewModel[] {
    const searchTerm = this.filterService.searchTerm();
    const hideCompleted = this.hideCompleted();

    return operations
      .filter(o =>
        (!searchTerm || this.isMatch(o, searchTerm)) &&
        (o.model.classification !== OperationStateClassification.Completed || !hideCompleted)
      )
      .sort((a, b) => {
        // Primary sort by sortOrder
        const orderDiff = (a.model.sortOrder ?? 0) - (b.model.sortOrder ?? 0);
        if (orderDiff !== 0) {
          return orderDiff;
        }

        // Secondary sort by plannedStart - (works because ISO date strings sort lexicographically)
        const startA = a.model.plannedStart ?? '';
        const startB = b.model.plannedStart ?? '';
        return startA.localeCompare(startB);
      });
  }

  private isMatch(operation: OperationViewModel, searchTerm: string) : boolean {
    const term = searchTerm.toLowerCase();
    return (operation.model.order?.toLowerCase().includes(term) ||
      operation.model.number?.toLowerCase().includes(term) ||
      operation.model.productIdentifier?.toLowerCase().includes(term) ||
      operation.model.productName?.toLowerCase().includes(term) ||
      operation.model.stateDisplayName?.toLowerCase().includes(term)) ?? false;
  }

  protected async onBegin(operation: OperationViewModel) {
    const context = await this.orderManagementService
      .getBeginContext({guid: operation.model.identifier!})
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
    const beginDialog = this.dialog.open(BeginDialog, {
      data: <BeginDialogData>{
        context: context,
        operation: operation,
      }
    });
    const beginModel = await firstValueFrom(beginDialog.afterClosed());
    if (!beginModel || !operation.model.identifier) {
      return;
    }

    this.orderManagementService
      .beginOperation({
        guid: operation.model.identifier,
        body: beginModel
      })
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
  }

  protected onInterrupt(operation: OperationViewModel) {
    this.dialog.open(InterruptDialog, {
      data: <InterruptDialogData>{
        operation: operation,
        onSubmit: this.submitInterruption.bind(this),
      }
    });
  }

  private submitInterruption(guid: string): Promise<void> {
    return this.orderManagementService.interruptOperation({
      guid: guid
    });
  }

  protected onReport(operation: OperationViewModel) {
    this.dialog.open(ReportDialog, {
      data: <ReportDialogData>{
        operation: operation,
        isReport: true,
        onGetContext: this.getReportContext.bind(this),
        onSubmit: this.submitReport.bind(this),
      }
    });
  }

  private getReportContext(guid: string): Promise<ReportContext> {
    return this.orderManagementService.getReportContext({guid: guid});
  }

  private submitReport(guid: string, body: ReportModel): Promise<void> {
    return this.orderManagementService.reportOperation({
      guid: guid,
      body: body
    });
  }

  protected onCreate() {
    this.dialog.open(CreateDialog);
  }

  protected async onAssign(operation: OperationViewModel) {
    await this.orderManagementService
      .reload({guid: operation.model.identifier!})
      .catch(() => this.snackbarService.showError(this.translateService.instant(TranslationConstants.OPERATIONS.REASSIGN_NOT_POSSIBLE)));
  }

  protected showRecipes(operation: OperationViewModel) {
    const identifier: string = `${operation.model.identifier}`;
    this.router.navigate(['operation-recipes', identifier]);
  }

  protected showDocuments(operation: OperationViewModel) {
    const identifier: string = `${operation.model.identifier}`;
    this.router.navigate(['operation-documents', identifier]);
  }

  protected onShowMessages(operationViewModel: OperationViewModel) {
    this.modifyDrawer(operationViewModel.model, DrawerContent.Messages);
  }

  protected onToggleFilter() {
    if (this.drawerContent() === DrawerContent.Filter) {
      this.closeDrawer();
    } else  {
      this.drawerContent.set(DrawerContent.Filter);
      this.drawer().open();
    }
  }

  private modifyDrawer(operation: OperationModel, targetContent: DrawerContent) {
    if (this.drawerContent() === DrawerContent.None) {
      this.selectedOperation.set(operation);
      this.drawerContent.set(targetContent);
      this.drawer().open();
    } else if (this.drawerContent() === targetContent) {
      this.closeDrawer();
    } else {
      this.selectedOperation.set(operation);
      this.drawerContent.set(targetContent);
    }
  }

  private subscribeForMessagesCount(operation: OperationViewModel) {
    this.orderManagementService.getLogs({guid: operation.model.identifier!}).then(
      messages =>
        (operation.errorMessagesCount = messages.filter(
          m => m.logLevel === LogLevel.Error || m.logLevel == LogLevel.Critical
        ).length)
    );
  }

  protected onShowPartList(operation: OperationViewModel) {
    this.modifyDrawer(operation.model, DrawerContent.Parts);
  }

  protected onToggleSource(operation: OperationViewModel) {
    this.modifyDrawer(operation.model, DrawerContent.Source);
  }

  protected onPanelExpandedChange(isExpanded: boolean, operation: OperationViewModel): void {
    if (isExpanded) {
      // Panel is being expanded
      this.selectedOperation.set(operation.model);
      this.selectedOperationId.set(operation.model.identifier);
      this.updateQueryParam(operation.model.identifier);
    } else {
      // Panel is being collapsed
      this.onPanelCollapse(operation);
    }
  }

  private onPanelCollapse(operation: OperationViewModel) {
    if (this.selectedOperation()?.identifier === operation.model.identifier) {
      // User clicked to close the panel
      this.selectedOperation.set(undefined);
      this.selectedOperationId.set(undefined);
      this.updateQueryParam(undefined);
      if (this.drawerContent() !== DrawerContent.Filter) {
        this.closeDrawer();
      }
    } else {
      // Panel collapsed because another was opened
    }
  }

  protected closeDrawer() {
    this.drawerContent.set(DrawerContent.None);
    this.drawer().close();
  }

  protected disconnectEvents(): void {
    this.mobileQuery.removeEventListener('change', this._mobileQueryListener);
    this.searchBarService.unsubscribe();
    this.operationService.disconnect();
  }
}

