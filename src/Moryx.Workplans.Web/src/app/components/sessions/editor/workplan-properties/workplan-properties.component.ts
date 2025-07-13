import { Component, OnDestroy } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { WorkplanState } from '../../../../api/models';
import { TranslationConstants } from '../../../../extensions/translation-constants.extensions';
import { EditorStateService } from '../../../../services/editor-state.service';
import { SessionsService } from '../../../../services/sessions.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

@Component({
    selector: 'app-workplan-properties',
    templateUrl: './workplan-properties.component.html',
    styleUrls: ['./workplan-properties.component.scss'],
    standalone: true,
    imports: 
    [
      CommonModule,
      FormsModule,
      MatFormFieldModule,
      MatInputModule,
      MatSelectModule,
      TranslateModule,
      
    ]
})
export class WorkplanPropertiesComponent implements OnDestroy {
  TranslationConstants = TranslationConstants;
  readonly WorkplanStates = Object.values(WorkplanState);

  constructor(
    public translate: TranslateService,
    public editorState: EditorStateService,
    private sessionService: SessionsService
  ) {}

  ngOnDestroy(): void {
    if (this.editorState.workplan)
      this.sessionService.updateSession(this.editorState.workplan);
  }
}
