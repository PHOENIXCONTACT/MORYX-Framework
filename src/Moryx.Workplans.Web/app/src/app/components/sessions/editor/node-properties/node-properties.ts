/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, effect, inject, signal, untracked, ChangeDetectionStrategy } from '@angular/core';
import { Entry, NavigableEntryEditor, PrototypeToEntryConverter } from '@moryx/ngx-web-framework/entry-editor';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslatePipe } from '@ngx-translate/core';
import { WorkplanNodeClassification, WorkplanNodeModel } from '@api/models';
import { WorkplanEditingService } from '@api/services';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { SessionsService } from '@app/services/sessions.service';
import { EditorStateService } from '@app/services/editor-state.service';

import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-node-properties',
  templateUrl: './node-properties.html',
  styleUrls: ['./node-properties.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatSelectModule,
    FormsModule,
    NavigableEntryEditor,
    TranslatePipe,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule
  ]
})
export class NodeProperties {
  private sessionsService = inject(SessionsService);
  private workplanEditingService = inject(WorkplanEditingService);
  private snackbarService = inject(SnackbarService);
  private editorStateService = inject(EditorStateService);

  protected node = signal<WorkplanNodeModel | undefined>(undefined);
  protected properties = signal<Entry | undefined>(undefined);

  protected readonly workplanNodeClassification = WorkplanNodeClassification;
  protected readonly TranslationConstants = TranslationConstants;

  constructor() {
    effect(() => {
      const step = this.editorStateService.isEditingStep();
      // Awaiting this results in a race condition,
      // this.node needs to be set before the observable provides the next value
      untracked(() => {
        const currentNode = this.node();
        if (currentNode) {
          this.updateNode(currentNode);
        }

        this.node.set(step);
        this.properties.set(step?.properties?.subEntries?.find(p => p.identifier === 'Parameters'));
      });
    });
  }

  private async updateNode(node: WorkplanNodeModel) {
    const activeSession = this.sessionsService.activeSession();
    if (!activeSession || !node.id || !this.editorStateService.workplan) {
      return;
    }

    if (node.properties) {
      PrototypeToEntryConverter.convertToEntry(node.properties);
    }

    await this.workplanEditingService
      .updateStep({sessionId: activeSession, nodeId: node.id, body: node})
      .then(updatedNode => {
        if (!this.editorStateService.workplan) {
          return;
        }
        const newNodes = this.editorStateService.workplan.nodes?.filter(keep => keep.id != updatedNode?.id);
        if (newNodes?.length === this.editorStateService.workplan.nodes?.length) {
          return;
        }
        this.editorStateService.workplan.nodes = newNodes;
        this.editorStateService.workplan.nodes?.push(updatedNode);
        this.sessionsService.registerUpdatedSession(this.editorStateService.workplan);
        this.editorStateService.notifyWorkplanChanged();
      })
      .catch(async (e: HttpErrorResponse) => {
        await this.snackbarService.handleError(e);
        this.node.set(node);
      });
  }

  protected onNavigateClick() {
    const subworkplanId = this.node()?.subworkplanId;
    if (!subworkplanId) {
      return;
    }
    this.sessionsService
      .getSessionForWorkplan(subworkplanId)
      .then(session => this.sessionsService.activateSession(session.sessionToken!))
      .catch((err: HttpErrorResponse) => this.snackbarService.handleError(err));
  }
}

