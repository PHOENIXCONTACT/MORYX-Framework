/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnDestroy, signal } from '@angular/core';
import { Entry, NavigableEntryEditor, PrototypeToEntryConverter } from '@moryx/ngx-web-framework/entry-editor';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslateModule } from '@ngx-translate/core';
import { WorkplanNodeClassification, WorkplanNodeModel } from '../../../../api/models';
import { WorkplanEditingService } from '../../../../api/services';
import { TranslationConstants } from '../../../../extensions/translation-constants.extensions';
import { SessionsService } from '../../../../services/sessions.service';
import { EditorStateService } from '../../../../services/editor-state.service';
import { Subscription } from 'rxjs';

import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-node-properties',
  templateUrl: './node-properties.html',
  styleUrls: ['./node-properties.scss'],
  imports: [
    MatSelectModule,
    FormsModule,
    NavigableEntryEditor,
    TranslateModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule
  ]
})
export class NodeProperties implements OnDestroy {
  private sessionsService = inject(SessionsService);
  private workplanEditingService = inject(WorkplanEditingService);
  private snackbarService = inject(SnackbarService);
  private editorStateService = inject(EditorStateService);

  node = signal<WorkplanNodeModel | undefined>(undefined);
  properties = signal<Entry | undefined>(undefined);

  readonly workplanNodeClassification = WorkplanNodeClassification;
  readonly TranslationConstants = TranslationConstants;

  private subscriptions: Subscription[] = [];
  private activeSession: string | undefined;

  constructor() {
    this.subscriptions.push(this.sessionsService.activeSession$.subscribe(s => (this.activeSession = s)));
    this.subscriptions.push(
      this.editorStateService.isEditingStep$.subscribe(async step => {
        // Awaiting this results in a race condition,
        // this.node needs to be set before the observable provides the next value
        if (this.node()) this.updateNode(this.node()!);

        this.node.update(_ => step);
        this.properties.update(_ => step?.properties?.subEntries?.find(p => p.identifier === 'Parameters'));
      })
    );
  }

  async updateNode(node: WorkplanNodeModel) {
    if (!this.activeSession || !node.id || !this.editorStateService.workplan) return;

    if (node.properties)
      PrototypeToEntryConverter.convertToEntry(node.properties);

    await this.workplanEditingService
      .updateStep({sessionId: this.activeSession, nodeId: node.id, body: node})
      .toAsync()
      .then(updatedNode => {
        if (!this.editorStateService.workplan) return;
        const newNodes = this.editorStateService.workplan.nodes?.filter(keep => keep.id != updatedNode?.id);
        if (newNodes?.length === this.editorStateService.workplan.nodes?.length) return;
        this.editorStateService.workplan.nodes = newNodes;
        this.editorStateService.workplan.nodes?.push(updatedNode);
        this.sessionsService.registerUpdatedSession(this.editorStateService.workplan);
        this.editorStateService.workplanChanged();
      })
      .catch(async (e: HttpErrorResponse) => {
        await this.snackbarService.handleError(e);
        this.node.update(_ => node);
      });
  }

  async ngOnDestroy(): Promise<void> {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  onNavigateClick() {
    if (!this.node()?.subworkplanId) return;
    this.sessionsService
      .getSessionForWorkplan(this.node()?.subworkplanId!)
      .toAsync()
      .then(session => this.sessionsService.activateSession(session.sessionToken!))
      .catch(async (err: HttpErrorResponse) => await this.snackbarService.handleError(err));
  }
}

