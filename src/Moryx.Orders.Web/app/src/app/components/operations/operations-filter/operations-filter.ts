import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { FilterService } from '@app/services/filter.service';
import { toSignal } from '@angular/core/rxjs-interop';
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

  hideCompleted = toSignal(this.filterService.hideCompleted$, { initialValue: true });

  TranslationConstants = TranslationConstants;

  toggleHideCompleted(): void {
    this.filterService.toggleHideCompleted();
  }
}
