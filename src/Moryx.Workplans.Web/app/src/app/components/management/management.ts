/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, effect, inject, OnInit, signal, ChangeDetectionStrategy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import { SnackbarService, SearchBarService, SearchRequest, SearchSuggestion } from '@moryx/ngx-web-framework/services';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';
import { WorkplanModel, WorkplanSessionModel } from '@api/models';
import { WorkplanService } from '@api/services';
import { ConfirmDialog, ConfirmDialogData } from '@app/dialogs/dialog-confirm/dialog-confirm';
import '../../extensions/array.extensions';

import { SessionsService } from '@app/services/sessions.service';
import { TranslationConstants } from '@app/translation-constants';

import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-management',
  templateUrl: './management.html',
  styleUrls: ['./management.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatTableModule,
    MatTooltipModule,
    MatIconModule,
    MatProgressSpinnerModule,
    TranslatePipe,
    MatButtonModule,
    MatCardModule
  ]
})
export class Management implements OnInit {
  private workplanService = inject(WorkplanService);
  private sessionService = inject(SessionsService);
  private snackbarService = inject(SnackbarService);
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);
  private location = inject(Location);
  private searchBarService = inject(SearchBarService);
  private dialog = inject(MatDialog);
  private translateService = inject(TranslateService);
  private destroyRef = inject(DestroyRef);

  protected TranslationConstants = TranslationConstants;
  protected readonly displayedColumns: string[] = ['name', 'state', 'version', 'actions'];

  protected workplans = signal<WorkplanModel[]>([]);
  protected sessions = signal<WorkplanSessionModel[]>([]);
  protected isLoading = signal(false);

  protected dataSource!: MatTableDataSource<WorkplanModel>;

  constructor() {
    effect(() => {
      const tokens = this.sessionService.availableSessions();
      this.onSessionsChanged(tokens);
    });
    this.destroyRef.onDestroy(() => this.searchBarService.unsubscribe());
  }

  ngOnInit(): void {
    this.isLoading.set(true);
    this.workplanService.getAllWorkplans().then(workplans => {
      this.workplans.set(workplans);
      this.dataSource = new MatTableDataSource<WorkplanModel>(this.workplans());
      this.isLoading.set(false);
      this.openFromQueryParam();
    }).catch(async (e: HttpErrorResponse) => {
      await this.snackbarService.handleError(e);
      this.isLoading.set(false);
    });

    this.searchBarService.subscribe({
      next: (request: SearchRequest) => this.onSearch(request),
    });
  }

  private async onSessionsChanged(tokens: string[]): Promise<void> {
    this.sessions.set([]);
    await Promise.all(
      tokens.map(
        async token =>
          await this.sessionService
            .getSession(token)
            .then((value: WorkplanSessionModel) => this.sessions.update(items => {
              items.push(value);
              return items;
            }))
            .catch((err: HttpErrorResponse) => this.snackbarService.handleError(err))
      )
    );
  }

  private openFromQueryParam() {
    const openId = this.activatedRoute.snapshot.queryParamMap.get('open');
    if (openId) {
      const workplan = this.workplans().find(w => w.id === Number(openId));
      if (workplan) {
        this.onOpenSession(workplan);
      }
    }
  }

  private onSearch(request: SearchRequest) {
    const matches = this.workplans().filter(w =>
      w.name?.toLowerCase().includes(request.term.toLowerCase())
    );

    if (request.submitted) {
      this.searchBarService.clearSuggestions();
      if (matches.length === 1) {
        this.onOpenSession(matches[0]);
      }
      this.searchBarService.subscribe({
        next: (newRequest: SearchRequest) => this.onSearch(newRequest),
      });
    } else {
      const suggestions: SearchSuggestion[] = matches
        .filter(w => w.name)
        .map(w => ({
          text: w.name!,
          url: this.location.prepareExternalUrl(this.router.serializeUrl(
            this.router.createUrlTree(['management'], { queryParams: { open: w.id } })
          )),
        }));
      this.searchBarService.provideSuggestions(suggestions);
    }
  }

  private async getTranslations(): Promise<{ [key: string]: string }> {
    return await firstValueFrom(this.translateService
      .get([
        TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.MESSAGE_FIRST_PART,
        TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.MESSAGE_SECOND_PART,
        TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.MESSAGE,
        TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.TITLE,
        TranslationConstants.MANAGEMENT.SNACK_BAR.SUCCESS_FIRST_PART,
        TranslationConstants.MANAGEMENT.SNACK_BAR.SUCCESS_SECOND_PART
      ]));
  }

  protected onDeleteWorkplan(workplan: WorkplanModel) {
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

    const confirmDialog = this.dialog.open(ConfirmDialog, {
      data: <ConfirmDialogData>{
        title: translations[TranslationConstants.MANAGEMENT.CONFRIM_DIALOG.TITLE],
        messages: [dialogMessage],
      }
    });

    confirmDialog.afterClosed().subscribe(async (confirmed) => {
      if (confirmed) {
        await this.workplanService.deleteWorkplan({id: workplan?.id ?? 0}).then(() => {
          this.completeTheDeletion(session, workplan, translations);
        }).catch((err: HttpErrorResponse) => this.snackbarService.handleError(err));
      }
    });
  }

  private completeTheDeletion(
    session: WorkplanSessionModel | undefined,
    workplan: WorkplanModel,
    translations: { [key: string]: string }
  ) {
    if (session) {
      this.sessionService.closeSession(session.sessionToken!)
        .catch((err: HttpErrorResponse) => this.snackbarService.handleError(err));
    }

    if (!this.workplans().length) {
      return;
    }
    this.workplans.update(items => {
      items.remove(workplan);
      return items;
    });
    this.dataSource = new MatTableDataSource<WorkplanModel>(this.workplans());
    this.snackbarService.showSuccess(
      `${translations[TranslationConstants.MANAGEMENT.SNACK_BAR.SUCCESS_FIRST_PART]} "${workplan?.name}" ${
        translations[TranslationConstants.MANAGEMENT.SNACK_BAR.SUCCESS_SECOND_PART]
      }`
    );
  }

  protected onOpenSession(workplan: WorkplanModel) {
    this.sessionService
      .getSessionForWorkplan(workplan.id!)
      .then(session => this.openSession(session))
      .catch((err: HttpErrorResponse) => this.snackbarService.handleError(err));
  }

  protected onDuplicateWorkplan(workplan: WorkplanModel): void {
    this.sessionService
      .getSessionForWorkplan(workplan.id!, true)
      .then(session => this.openSession(session))
      .catch((err: HttpErrorResponse) => this.snackbarService.handleError(err));
  }

  private openSession(session: WorkplanSessionModel) {
    this.sessionService.activateSession(session.sessionToken!);
    this.router.navigate(['session', session.sessionToken]);
  }
}

