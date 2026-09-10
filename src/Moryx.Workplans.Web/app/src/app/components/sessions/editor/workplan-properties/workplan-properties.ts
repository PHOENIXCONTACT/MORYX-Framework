/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnDestroy, ChangeDetectionStrategy } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { WorkplanState } from '@api/models';
import { TranslationConstants } from '@app/translation-constants';
import { EditorStateService } from '@app/services/editor-state.service';
import { SessionsService } from '@app/services/sessions.service';

import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-workplan-properties',
  templateUrl: './workplan-properties.html',
  styleUrls: ['./workplan-properties.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    TranslatePipe
  ]
})
export class WorkplanProperties implements OnDestroy {
  protected translate = inject(TranslateService);
  protected editorState = inject(EditorStateService);
  private sessionService = inject(SessionsService);

  protected TranslationConstants = TranslationConstants;
  protected readonly WorkplanStates = Object.values(WorkplanState);

  ngOnDestroy(): void {
    if (this.editorState.workplan) {
      this.sessionService.updateSession(this.editorState.workplan);
    }
  }
}

