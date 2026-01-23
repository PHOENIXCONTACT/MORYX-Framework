/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnDestroy, signal } from '@angular/core';
import { Entry, MoryxSnackbarService, NavigableEntryEditorComponent, PrototypeToEntryConverter } from '@moryx/ngx-web-framework';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { WorkplanNodeClassification, WorkplanNodeModel } from '../../../../api/models';
import { WorkplanEditingService } from '../../../../api/services';
import { TranslationConstants } from '../../../../extensions/translation-constants.extensions';
import { SessionsService } from '../../../../services/sessions.service';
import { EditorStateService } from '../../../../services/editor-state.service';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

@Component({
    selector: 'app-node-properties',
    templateUrl: './node-properties.component.html',
    styleUrls: ['./node-properties.component.scss'],
    standalone: true,
    imports:
    [
      CommonModule,
      MatSelectModule,
      FormsModule,
      NavigableEntryEditorComponent,
      TranslateModule,
      MatButtonModule,
      MatFormFieldModule,
      MatInputModule
    ]
})
export class NodePropertiesComponent implements OnDestroy {
  node = signal<WorkplanNodeModel | undefined>(undefined);
  properties = signal<Entry | undefined>(undefined);

  readonly WorkplanNodeClassification = WorkplanNodeClassification;
  readonly TranslationConstants = TranslationConstants;

  private subscriptions: Subscription[] = [];
  private activeSession: string | undefined;

  constructor(
    private sessionService: SessionsService,
    private workplanEditing: WorkplanEditingService,
    private moryxSnackbar: MoryxSnackbarService,
    public translate: TranslateService,
    private editorState: EditorStateService
  ) {
    this.subscriptions.push(sessionService.activeSession$.subscribe(s => (this.activeSession = s)));
    this.subscriptions.push(
      editorState.isEditingStep$.subscribe(async step => {
        // Awaiting this results in a race condition, 
        // this.node needs to be set before the observable provides the next value
        if (this.node()) this.updateNode(this.node()!);

        this.node.update(_=> step);
        this.properties.update(_=> step?.properties?.subEntries?.find(p => p.identifier === 'Parameters'));
      })
    );
  }

  async updateNode(node: WorkplanNodeModel) {
    if (!this.activeSession || !node.id || !this.editorState.workplan) return;

    if (node.properties)
      PrototypeToEntryConverter.convertToEntry(node.properties);
    
    await this.workplanEditing
      .updateStep({ sessionId: this.activeSession, nodeId: node.id, body: node })
      .toAsync()
      .then(updatedNode => {
        if (!this.editorState.workplan) return;
        const newNodes = this.editorState.workplan.nodes?.filter(keep => keep.id != updatedNode?.id);
        if (newNodes?.length === this.editorState.workplan.nodes?.length) return;
        this.editorState.workplan.nodes = newNodes;
        this.editorState.workplan.nodes?.push(updatedNode);
        this.sessionService.registerUpdatedSession(this.editorState.workplan);
        this.editorState.workplanChanged();
      })
      .catch(async (e: HttpErrorResponse) => {
        await this.moryxSnackbar.handleError(e);
        this.node.update(_=> node);
      });
  }

  async ngOnDestroy(): Promise<void> {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  onNavigateClick() {
    if (!this.node()?.subworkplanId) return;
    this.sessionService
      .getSessionForWorkplan(this.node()?.subworkplanId!)
      .toAsync()
      .then(session => this.sessionService.activateSession(session.sessionToken!))
      .catch(async (err: HttpErrorResponse) => await this.moryxSnackbar.handleError(err));
  }
}

