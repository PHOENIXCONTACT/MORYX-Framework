/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { Router } from '@angular/router';
import { MoryxSnackbarService, SearchBarService, SearchRequest } from '@moryx/ngx-web-framework';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { WorkplanModel, WorkplanSessionModel } from '../../api/models';
import { WorkplanService } from '../../api/services';
import {
  ConfirmDialogButton,
  ConfirmDialogComponent,
  ConfirmDialogData,
} from '../../dialogs/dialog-confirm/dialog-confirm.component';
import '../../extensions/array.extensions';
import '../../extensions/observable.extensions';
import { SessionsService } from '../../services/sessions.service';
import { TranslationConstants } from '../../extensions/translation-constants.extensions';
import { Subscription } from 'rxjs';

import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

@Component({
    selector: 'app-management',
    templateUrl: './management.component.html',
    styleUrls: ['./management.component.scss'],
    standalone: true,
    imports: [
    MatTableModule,
    MatTooltipModule,
    MatIconModule,
    MatProgressSpinnerModule,
    TranslateModule,
    MatButtonModule,
    MatCardModule
]
})
export class ManagementComponent implements OnInit, OnDestroy {
  TranslationConstants = TranslationConstants;
  private availableSessionsSubscription?: Subscription;
  readonly displayedColumns: string[] = ['name', 'state', 'version', 'actions'];

  workplans = signal<WorkplanModel[]>([]);
  sessions = signal<WorkplanSessionModel[]>([]);
  isLoading = signal(false);

  dataSource!: MatTableDataSource<WorkplanModel>;

  constructor(
    private workplanService: WorkplanService,
    private sessionService: SessionsService,
    private moryxSnackbar: MoryxSnackbarService,
    private router: Router,
    private searchBarService: SearchBarService,
    public dialog: MatDialog,
    public translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.availableSessionsSubscription = this.sessionService.availableSessions$.subscribe(
      async tokens => await this.onSessionsChanged(tokens)
    );
    this.isLoading.update(_=> true);
    this.workplanService.getAllWorkplans().subscribe({
      next: workplans => {
        this.workplans.update(_=> workplans);
        this.dataSource = new MatTableDataSource<WorkplanModel>(this.workplans());
        this.isLoading.update(_=> false);
      },
      error: async (e: HttpErrorResponse) => {
        await this.moryxSnackbar.handleError(e);
        this.isLoading.update(_=> false);
      },
    });

    this.searchBarService.subscribe({
      next: this.onSearch,
    });
  }

  private async onSessionsChanged(tokens: string[]): Promise<void> {
    this.sessions.update(_=>[]);
    await Promise.all(
      tokens.map(
        async token =>
          await this.sessionService
            .getSession(token)
            .toAsync()
            .then((value: WorkplanSessionModel) => this.sessions.update(items =>{
              items.push(value)
              return items;
            }))
            .catch(async (err: HttpErrorResponse) => await this.moryxSnackbar.handleError(err))
      )
    );
  }

  private onSearch(request: SearchRequest) {
    if (!this.workplans().length) return;

    let workplans = this.workplans().filter(w => w.name?.includes(request.term));
    if (!workplans) workplans = [];

    if (request.submitted) {
      this.dataSource = new MatTableDataSource<WorkplanModel>(this.workplans());
      this.searchBarService.clearSuggestions();
      this.searchBarService.subscribe({
        next: (newRequest: SearchRequest) => {
          this.onSearch(newRequest);
        },
      });
    } else {
      this.dataSource = new MatTableDataSource<WorkplanModel>(
        this.workplans().filter(w => w.name?.toLowerCase().includes(request.term.toLowerCase()))
      );
    }
  }

  ngOnDestroy(): void {
    this.searchBarService.unsubscribe();
    this.availableSessionsSubscription?.unsubscribe();
  }

  async getTranslations(): Promise<{ [key: string]: string }> {
    return await this.translate
      .get([
        TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.MESSAGE_FIRST_PART,
        TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.MESSAGE_SECOND_PART,
        TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.MESSAGE,
        TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.TITLE,
        TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.CANCEL,
        TranslationConstants.MANAGEMENT.SNACK_BAR.SUCCESS_FIRST_PART,
        TranslationConstants.MANAGEMENT.SNACK_BAR.SUCCESS_SECOND_PART,
      ])
      .toAsync();
  }

  onDeleteWorkplan(workplan: WorkplanModel) {
    const session = this.sessions().find(s => s.workplanId === workplan.id);
    this.openConfirmDialog(session, workplan);
  }

  private async openConfirmDialog(session: WorkplanSessionModel | undefined, workplan: WorkplanModel) {
    const translations = await this.getTranslations();
    const dialogMessage = session
      ? `${translations[TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.MESSAGE_FIRST_PART]} "${session.name}" ${
          translations[TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.MESSAGE_SECOND_PART]
        }?`
      : translations[TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.MESSAGE];

    const confirmDialog = this.dialog.open(ConfirmDialogComponent, {
      data: <ConfirmDialogData>{
        title: translations[TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.TITLE],
        message: dialogMessage,
        buttons: [
          <ConfirmDialogButton>{
            text: translations[TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.CANCEL],
            action: () => confirmDialog.close(),
          },
          <ConfirmDialogButton>{
            text: 'Ok', // ToDo: internationalize
            action: () => {
              this.workplanService.deleteWorkplan({ id: workplan?.id! }).subscribe({
                next: () => {
                  this.completeTheDeletion(session, workplan, translations);
                  confirmDialog.close();
                },
                error: async (err: HttpErrorResponse) => await this.moryxSnackbar.handleError(err),
              });
            },
          },
        ],
      },
    });
  }

  private completeTheDeletion(
    session: WorkplanSessionModel | undefined,
    workplan: WorkplanModel,
    translations: { [key: string]: string }
  ) {
    if (session) {
      this.sessionService.closeSession(session.sessionToken!).subscribe({
        error: async (err: HttpErrorResponse) => await this.moryxSnackbar.handleError(err),
      });
    }

    if (!this.workplans().length) return;
    this.workplans.update(items =>{
      items.remove(workplan);
      return items;
    } )
    this.dataSource = new MatTableDataSource<WorkplanModel>(this.workplans());
    this.moryxSnackbar.showSuccess(
      `${translations[TranslationConstants.MANAGEMENT.SNACK_BAR.SUCCESS_FIRST_PART]} "${workplan?.name}" ${
        translations[TranslationConstants.MANAGEMENT.SNACK_BAR.SUCCESS_SECOND_PART]
      }`
    );
  }

  onOpenSession(workplan: WorkplanModel) {
    this.sessionService
      .getSessionForWorkplan(workplan.id!)
      .toAsync()
      .then(session => this.openSession(session))
      .catch(async (err: HttpErrorResponse) => await this.moryxSnackbar.handleError(err));
  }

  onDuplicateWorkplan(workplan: WorkplanModel): void {
    this.sessionService
      .getSessionForWorkplan(workplan.id!, true)
      .toAsync()
      .then(session => this.openSession(session))
      .catch(async (err: HttpErrorResponse) => await this.moryxSnackbar.handleError(err));
  }

  private openSession(session: WorkplanSessionModel) {
    this.sessionService.activateSession(session.sessionToken!);
    this.router.navigate(['session', session.sessionToken]);
  }
}

