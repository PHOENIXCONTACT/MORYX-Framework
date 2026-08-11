import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { FilterService } from '@app/services/filter.service';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { TranslatePipe } from '@ngx-translate/core';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';

@Component({
  selector: 'app-operations-filter',
  imports: [
    MatSlideToggleModule,
    TranslatePipe
  ],
  templateUrl: './operations-filter.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './operations-filter.scss',
})
export class OperationsFilter {
  private filterService = inject(FilterService);

  protected hideCompleted = this.filterService.hideCompleted;

  protected TranslationConstants = TranslationConstants;

  protected toggleHideCompleted(): void {
    this.filterService.toggleHideCompleted();
  }
}
