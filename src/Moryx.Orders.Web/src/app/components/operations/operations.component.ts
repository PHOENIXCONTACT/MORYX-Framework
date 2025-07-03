import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectorRef, Component, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatDrawer, MatSidenavModule } from '@angular/material/sidenav';
import { Router } from '@angular/router';
import { EmptyStateComponent, MoryxSnackbarService } from '@moryx/ngx-web-framework';
import { SearchBarService, SearchRequest } from '@moryx/ngx-web-framework/shell-services';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { OperationService } from 'src/app/services/operation.service';
import { OrderManagementService } from '../../api/services/order-management.service';
import { BeginDialogComponent, BeginDialogData } from '../../dialogs/begin-dialog/begin-dialog.component';
import { CreateDialogComponent } from '../../dialogs/create-dialog/create-dialog.component';
import { ReportDialogComponent, ReportDialogData } from '../../dialogs/report-dialog/report-dialog.component';
import '../../extensions/observable.extensions';
import { OperationViewModel } from '../../models/operation-view-model';
import { OperationModel } from 'src/app/api/models/Moryx/Orders/Endpoints/operation-model';
import { ReportModel } from 'src/app/api/models/Moryx/Orders/Endpoints/report-model';
import { OperationClassification } from 'src/app/api/models/Moryx/Orders/operation-classification';
import { ReportContext } from 'src/app/api/models/Moryx/Orders/report-context';
import { LogLevel } from 'src/app/api/models/Microsoft/Extensions/Logging/log-level';
import { MediaMatcher } from '@angular/cdk/layout';
import { DrawerContent } from './drawer-content';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { LogMessageListComponent } from './log-message-list/log-message-list.component';
import { PartListComponent } from './part-list/part-list.component';
import { OperationSourceComponent } from './operation-source/operation-source.component';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { MatBadgeModule } from '@angular/material/badge';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-operations',
  templateUrl: './operations.component.html',
  styleUrls: ['./operations.component.scss'],
  imports: [
    CommonModule,
    TranslateModule,
    MatIconModule,
    MatDrawer,
    MatSidenavModule,
    LogMessageListComponent,
    PartListComponent,
    OperationSourceComponent,
    MatExpansionModule,
    MatButtonModule,
    MatBadgeModule,
    EmptyStateComponent,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  standalone:true
})
export class OperationsComponent implements OnInit, OnDestroy {
  operations = signal<OperationViewModel[]>([]);
  DrawerContent = DrawerContent;
  drawerContent = signal<DrawerContent>(DrawerContent.None);
  selectedOperation = signal<OperationModel | undefined>(undefined);
  TranslationConstants = TranslationConstants;
  OperationClassification = OperationClassification;
  isLoading = signal<boolean>(true);
  mobileQuery: MediaQueryList;
  private searchTerm = signal<string>('');
  @ViewChild('drawer') public drawer!: MatDrawer;

  constructor(
    public orderManagementService: OrderManagementService,
    public dialog: MatDialog,
    private router: Router,
    private searchbar: SearchBarService,
    public translate: TranslateService,
    private moryxSnackbar: MoryxSnackbarService,
    private operationService: OperationService,
    changeDetectorRef: ChangeDetectorRef, 
    media: MediaMatcher
  ) {
    this.mobileQuery = media.matchMedia('(max-width: 1279px)');
    this._mobileQueryListener = () => changeDetectorRef.detectChanges();
    this.mobileQuery.addEventListener('change', this._mobileQueryListener);
  }
  
  private _mobileQueryListener: () => void;

  ngOnDestroy(): void {
    this.mobileQuery.removeEventListener('change', this._mobileQueryListener);
    this.searchbar.unsubscribe();
  }

  ngOnInit() {
    // Get all the operations
    this.orderManagementService.getOperations().subscribe({
      next: (operationResponse: OperationModel[]) => {
        this.operations.set(operationResponse
          .filter(operation => operation.classification !== OperationClassification.Completed)
          .map(model => {
            var viewModel = new OperationViewModel(model);
            this.subscribeForMessagesCount(viewModel);
            return viewModel;
          }))
        this.isLoading.set(false);
      },
      error: async (err: HttpErrorResponse) => {
        await this.moryxSnackbar.handleError(err);
        this.isLoading.set(false);
      },
    });

    // Register events
    this.operationService.operationChanged((updatedOperation: OperationModel) => {
      if (!updatedOperation) return;
      //filter the list after the update of a completed operation
      if (updatedOperation?.classification === OperationClassification.Completed) {
        this.operations.update(operations => operations.filter(
          operation => operation.model.identifier !== updatedOperation.identifier
        ));
        return;
      }
      var existent = this.operations().find(o => o.model.identifier == updatedOperation.identifier);
      if (existent) {
        existent.updateModel(updatedOperation);
      } else {
        this.operations.update(operations => {
          operations.push(new OperationViewModel(updatedOperation));
          return operations
        });
      }
    });

    // Searchbar
    this.searchbar.subscribe({
      next: (request: SearchRequest) => {
        this.onSearch(request);
      },
    });
  }

  onSearch(request: SearchRequest) {
    if (request.submitted) {
      this.searchbar.clearSuggestions();
      this.searchTerm.set('');
      this.searchbar.subscribe({
        next: (newRequest: SearchRequest) => {
          this.onSearch(newRequest);
        },
      });
    } else this.searchTerm.set(request.term);
  }

  filteringOperations(operations: OperationViewModel[]): OperationViewModel[] {
    var filteredOperations = <OperationViewModel[]>[];
    if (!this.searchTerm()) filteredOperations = operations;
    else filteredOperations = operations.filter(o => o.model.order?.includes(this.searchTerm()));
    return filteredOperations;
  }

  async onBegin(operation: OperationViewModel) {
    const context = await this.orderManagementService
      .getBeginContext({ guid: operation.model.identifier! })
      .toAsync()
      .catch(async (e: HttpErrorResponse) => await this.moryxSnackbar.handleError(e));
    const beginDialog = this.dialog.open(BeginDialogComponent, {
      data: <BeginDialogData>{
        context: context,
        operation: operation,
      },
    });
    const beginModel = await beginDialog.afterClosed().toAsync();
    if (!beginModel || !operation.model.identifier) return;

    this.orderManagementService
      .beginOperation({
        guid: operation.model.identifier,
        body: beginModel,
      })
      .subscribe({
        error: async (e: HttpErrorResponse) => await this.moryxSnackbar.handleError(e),
      });
  }

  onInterrupt(operation: OperationViewModel) {
    this.dialog.open(ReportDialogComponent, {
      data: <ReportDialogData>{
        operation: operation,
        isReport: false,
        onGetContext: this.getInterruptContext.bind(this),
        onSubmit: this.submitInterruption.bind(this),
      },
    });
  }

  private getInterruptContext(guid: string): Observable<ReportContext> {
    return this.orderManagementService.getInterruptContext({ guid: guid });
  }

  private submitInterruption(guid: string, body: ReportModel): Observable<void> {
    return this.orderManagementService.interruptOperation({
      guid: guid,
      body: body,
    });
  }

  onReport(operation: OperationViewModel) {
    this.dialog.open(ReportDialogComponent, {
      data: <ReportDialogData>{
        operation: operation,
        isReport: true,
        onGetContext: this.getReportContext.bind(this),
        onSubmit: this.submitReport.bind(this),
      },
    });
  }

  private getReportContext(guid: string): Observable<ReportContext> {
    return this.orderManagementService.getReportContext({ guid: guid });
  }

  private submitReport(guid: string, body: ReportModel): Observable<void> {
    return this.orderManagementService.reportOperation({
      guid: guid,
      body: body,
    });
  }

  onCreate() {
    this.dialog.open(CreateDialogComponent);
  }

  async onAssign(operation: OperationViewModel) {
    await this.orderManagementService
      .reload({ guid: operation.model.identifier! })
      .toAsync()
      .catch(async (e: HttpErrorResponse) => await this.moryxSnackbar.showError(this.translate.instant(TranslationConstants.OPERATIONS.REASSIGN_NOT_POSSIBLE)));
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

  modifyDrawer(operation: OperationModel, targetContent: DrawerContent) {
    if(this.drawerContent() === DrawerContent.None) {
      this.selectedOperation.set(operation);
      this.drawerContent.set(targetContent);
      this.drawer.open();
    } else if(this.drawerContent() === targetContent) {
      this.closeDrawer();
    } else {
      this.selectedOperation.set(operation);
      this.drawerContent.set(targetContent);  
    }
  }

  subscribeForMessagesCount(operation: OperationViewModel) {
    this.orderManagementService.getLogs({ guid: operation.model.identifier!}).subscribe(
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
      this.closeDrawer();
    } else {
      // Panel collapsed because another was opened
    }
  }

  closeDrawer() {
    this.drawerContent.set(DrawerContent.None);
    this.drawer.close();
  }
}
