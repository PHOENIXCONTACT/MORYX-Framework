/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectorRef, Component, inject, OnDestroy, OnInit, signal, viewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatDrawer, MatSidenavModule } from '@angular/material/sidenav';
import { Router } from '@angular/router';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { SearchBarService, SearchRequest } from '@moryx/ngx-web-framework/services';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { OperationService } from 'src/app/services/operation.service';
import { OrderManagementService } from '../../api/services/order-management.service';
import { BeginDialog, BeginDialogData } from '../../dialogs/begin-dialog/begin-dialog';
import { CreateDialog } from '../../dialogs/create-dialog/create-dialog';
import { ReportDialog, ReportDialogData } from '../../dialogs/report-dialog/report-dialog';
import { InterruptDialog } from '../../dialogs/interrupt-dialog/interrupt-dialog';
import { InterruptDialogData } from '../../dialogs/interrupt-dialog/interrupt-dialog-data';
import '../../extensions/observable.extensions';
import { OperationViewModel } from '../../models/operation-view-model';
import { OperationModel } from '../../api/models';
import { ReportModel } from '../../api/models';
import { OperationStateClassification } from '../../api/models';
import { ReportContext } from '../../api/models';
import { LogLevel } from '../../api/models';
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
import { FilterService } from '../../services/filter.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { OperationsFilter } from './operations-filter/operations-filter';
import { MultiProgressBar } from '../../multi-progress-bar/multi-progress-bar';

@Component({
  selector: 'app-operations',
  templateUrl: './operations.html',
  styleUrls: ['./operations.scss'],
  imports: [
    CommonModule,
    TranslateModule,
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
]
})
export class Operations implements OnInit, OnDestroy {
  private orderManagementService = inject(OrderManagementService);
  private dialog = inject(MatDialog);
  private router = inject(Router);
  private searchBarService = inject(SearchBarService);
  private translateService = inject(TranslateService);
  private snackbarService = inject(SnackbarService);
  private operationService = inject(OperationService);
  private changeDetectorRef = inject(ChangeDetectorRef);
  private mediaMatcher = inject(MediaMatcher);
  private filterService = inject(FilterService);

  operations = signal<OperationViewModel[]>([]);
  DrawerContent = DrawerContent;
  drawerContent = signal<DrawerContent>(DrawerContent.None);
  selectedOperation = signal<OperationModel | undefined>(undefined);
  TranslationConstants = TranslationConstants;
  OperationStateClassification = OperationStateClassification;
  isLoading = signal<boolean>(true);
  mobileQuery: MediaQueryList;
  private searchTerm = signal<string>('');
  drawer = viewChild.required<MatDrawer>('drawer');
  hideCompleted = toSignal(this.filterService.hideCompleted$, { initialValue: true });

  constructor() {
    this.mobileQuery = this.mediaMatcher.matchMedia('(max-width: 1279px)');
    this._mobileQueryListener = () => this.changeDetectorRef.detectChanges();
    this.mobileQuery.addEventListener('change', this._mobileQueryListener);
  }

  private readonly _mobileQueryListener: () => void;

  ngOnDestroy(): void {
    this.mobileQuery.removeEventListener('change', this._mobileQueryListener);
    this.searchBarService.unsubscribe();
  }

  ngOnInit() {
    // Get all the operations
    this.orderManagementService.getOperations().subscribe({
      next: (operationResponse: OperationModel[]) => {
        this.operations.set(operationResponse
          .map(model => {
            const viewModel = new OperationViewModel(model);
            this.subscribeForMessagesCount(viewModel);
            return viewModel;
          }))
        this.isLoading.set(false);
      },
      error: async (err: HttpErrorResponse) => {
        await this.snackbarService.handleError(err);
        this.isLoading.set(false);
      }
    });

    // Register events
    this.operationService.operationChanged((updatedOperation: OperationModel) => {
      if (!updatedOperation) {
        return;
      }

      const existent = this.operations().find(o => o.model.identifier == updatedOperation.identifier);
      if (existent) {
        existent.updateModel(updatedOperation);

        // TODO: This is a workaround to trigger change detection for the updated job.
        //  The OperationViewModel is mutable and Angular does not detect changes to its properties.
        //  Consider refactoring OperationViewModel to be immutable to avoid this issue.
        this.changeDetectorRef.markForCheck();
      } else {
        this.operations.update(operations => {
          operations.push(new OperationViewModel(updatedOperation));
          return operations
        });
      }
    });

    // Searchbar
    this.searchBarService.subscribe({
      next: (request: SearchRequest) => {
        this.onSearch(request);
      }
    });
  }

  onSearch(request: SearchRequest) {
    if (request.submitted) {
      this.searchBarService.clearSuggestions();
      this.searchTerm.set('');
      this.searchBarService.subscribe({
        next: (newRequest: SearchRequest) => {
          this.onSearch(newRequest);
        }
      });
    } else {
      this.searchTerm.set(request.term);
    }
  }

  filteringOperations(operations: OperationViewModel[]): OperationViewModel[] {
    const searchTerm = this.searchTerm();
    const hideCompleted = this.hideCompleted();

    return operations
      .filter(o =>
        (!searchTerm || o.model.order?.includes(searchTerm)) &&
        (o.model.classification !== OperationStateClassification.Completed || !hideCompleted)
      )
      .sort((a, b) => {
        // Primary sort by sortOrder
        const orderDiff = (a.model.sortOrder ?? 0) - (b.model.sortOrder ?? 0);
        if (orderDiff !== 0) return orderDiff;

        // Secondary sort by plannedStart - (works because ISO date strings sort lexicographically)
        const startA = a.model.plannedStart ?? '';
        const startB = b.model.plannedStart ?? '';
        return startA.localeCompare(startB);
      });
  }

  async onBegin(operation: OperationViewModel) {
    const context = await this.orderManagementService
      .getBeginContext({guid: operation.model.identifier!})
      .toAsync()
      .catch(async (e: HttpErrorResponse) => await this.snackbarService.handleError(e));
    const beginDialog = this.dialog.open(BeginDialog, {
      data: <BeginDialogData>{
        context: context,
        operation: operation,
      }
    });
    const beginModel = await beginDialog.afterClosed().toAsync();
    if (!beginModel || !operation.model.identifier) return;

    this.orderManagementService
      .beginOperation({
        guid: operation.model.identifier,
        body: beginModel
      })
      .subscribe({
        error: async (e: HttpErrorResponse) => await this.snackbarService.handleError(e)
      });
  }

  onInterrupt(operation: OperationViewModel) {
    this.dialog.open(InterruptDialog, {
      data: <InterruptDialogData>{
        operation: operation,
        onSubmit: this.submitInterruption.bind(this),
      }
    });
  }

  private submitInterruption(guid: string): Observable<void> {
    return this.orderManagementService.interruptOperation({
      guid: guid
    });
  }

  onReport(operation: OperationViewModel) {
    this.dialog.open(ReportDialog, {
      data: <ReportDialogData>{
        operation: operation,
        isReport: true,
        onGetContext: this.getReportContext.bind(this),
        onSubmit: this.submitReport.bind(this),
      }
    });
  }

  private getReportContext(guid: string): Observable<ReportContext> {
    return this.orderManagementService.getReportContext({guid: guid});
  }

  private submitReport(guid: string, body: ReportModel): Observable<void> {
    return this.orderManagementService.reportOperation({
      guid: guid,
      body: body
    });
  }

  onCreate() {
    this.dialog.open(CreateDialog);
  }

  async onAssign(operation: OperationViewModel) {
    await this.orderManagementService
      .reload({guid: operation.model.identifier!})
      .toAsync()
      .catch(async (e: HttpErrorResponse) => await this.snackbarService.showError(this.translateService.instant(TranslationConstants.OPERATIONS.REASSIGN_NOT_POSSIBLE)));
  }

  showRecipes(operation: OperationViewModel) {
    const identifier: string = `${operation.model.identifier}`;
    this.router.navigate(['operation-recipes', identifier]);
  }

  showDocuments(operation: OperationViewModel) {
    const identifier: string = `${operation.model.identifier}`;
    this.router.navigate(['operation-documents', identifier]);
  }

  onShowMessages(operationViewModel: OperationViewModel) {
    this.modifyDrawer(operationViewModel.model, DrawerContent.Messages);
  }

  onToggleFilter() {
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

  subscribeForMessagesCount(operation: OperationViewModel) {
    this.orderManagementService.getLogs({guid: operation.model.identifier!}).subscribe(
      messages =>
        (operation.errorMessagesCount = messages.filter(
          m => m.logLevel === LogLevel.Error || m.logLevel == LogLevel.Critical
        ).length)
    );
  }

  onShowPartList(operation: OperationViewModel) {
    this.modifyDrawer(operation.model, DrawerContent.Parts);
  }

  onToggleSource(operation: OperationViewModel) {
    this.modifyDrawer(operation.model, DrawerContent.Source);
  }

  onPanelExpandedChange(isExpanded: boolean, operation: OperationViewModel): void {
    if (isExpanded) {
      // Panel is being expanded
      this.selectedOperation.set(operation.model);
    } else {
      // Panel is being collapsed
      this.onPanelCollapse(operation);
    }
  }

  private onPanelCollapse(operation: OperationViewModel) {
    if (this.selectedOperation()?.identifier === operation.model.identifier) {
      // User clicked to close the panel
      this.selectedOperation.set(undefined);
      if (this.drawerContent() !== DrawerContent.Filter) {
        this.closeDrawer();
      }
    } else {
      // Panel collapsed because another was opened
    }
  }

  closeDrawer() {
    this.drawerContent.set(DrawerContent.None);
    this.drawer().close();
  }
}

