import { Component, inject } from '@angular/core';
import { MatCheckbox } from '@angular/material/checkbox';
import { FilterService } from '../../../services/filter.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { TranslationConstants } from '../../../extensions/translation-constants.extensions';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-operations-filter',
  imports: [
    MatCheckbox,
    TranslateModule
  ],
  templateUrl: './operations-filter.html',
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
