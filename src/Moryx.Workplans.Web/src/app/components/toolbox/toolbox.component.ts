import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { MoryxSnackbarService } from '@moryx/ngx-web-framework';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
import { WorkplanNodeClassification, WorkplanStepRecipe } from '../../api/models';
import { WorkplanEditingService } from '../../api/services';
import { TranslationConstants } from '../../extensions/translation-constants.extensions';
import { EditorStateService } from '../../services/editor-state.service';
import { SessionsService } from '../../services/sessions.service';
import { CommonModule } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

@Component({
    selector: 'app-toolbox',
    templateUrl: './toolbox.component.html',
    styleUrls: ['./toolbox.component.scss'],
    standalone: true,
    imports: [
      CommonModule,
      MatExpansionModule,
      MatIconModule,
      MatCardModule,
      TranslateModule,
    ]
})
export class ToolboxComponent implements OnInit, OnDestroy {
  constructor(
    private workplanEditing: WorkplanEditingService,
    public translate: TranslateService,
    private moryxSnackbar: MoryxSnackbarService,
    private sessionService: SessionsService
  ) {}

  subscription: Subscription | undefined;
  stepRecipes: WorkplanStepRecipe[] = [];
  TranslationConstants = TranslationConstants;

  ngOnInit(): void {
    this.subscription = this.sessionService.activeSession$.subscribe(changed => {
      if (changed) {
        this.getAvailableSteps();
      }
    });
  }

  // ToDo: Add cache for available steps somewhere. They are fetched multiple times and also in the editor component
  getAvailableSteps() {
    this.workplanEditing.availableSteps().subscribe({
      next: steps => (this.stepRecipes = steps),
      error: async (e: HttpErrorResponse) => await this.moryxSnackbar.handleError(e),
    });
  }

  handleDragStart(stepRecipe: WorkplanStepRecipe, event: DragEvent) {
    event.dataTransfer?.setData('string', stepRecipe.type ? JSON.stringify(stepRecipe) : '');
  }

  getStepIcon(classification: WorkplanNodeClassification | undefined) {
    switch (classification) {
      case WorkplanNodeClassification.Input:
        return 'arrow_circle_down';
      case WorkplanNodeClassification.Output:
        return 'arrow_circle_up';
      case WorkplanNodeClassification.Execution:
        return 'commit';
      case WorkplanNodeClassification.Subworkplan:
        return 'account_tree';
      case WorkplanNodeClassification.ControlFlow:
        return 'alt_route';
      default:
        return 'question_mark';
    }
  }

  getControlFlow(): WorkplanStepRecipe[] {
    return this.stepRecipes.filter(sr => sr.classification == WorkplanNodeClassification.ControlFlow);
  }

  getExecution(): WorkplanStepRecipe[] {
    return this.stepRecipes.filter(sr => sr.classification == WorkplanNodeClassification.Execution);
  }

  getSubworkplans(): WorkplanStepRecipe[] {
    return this.stepRecipes.filter(sr => sr.classification == WorkplanNodeClassification.Subworkplan);
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }
}
