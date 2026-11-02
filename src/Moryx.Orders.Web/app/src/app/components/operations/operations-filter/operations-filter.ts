import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { FilterService } from '@app/services/filter.service';
import { TranslationConstants } from '@app/translation-constants';
import { TranslatePipe } from '@ngx-translate/core';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-operations-filter',
  imports: [
    MatSlideToggleModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    TranslatePipe
  ],
  templateUrl: './operations-filter.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './operations-filter.scss',
})
export class OperationsFilter {
  private filterService = inject(FilterService);

  protected hideCompleted = this.filterService.hideCompleted;
  protected searchTerm = this.filterService.searchTerm;

  protected TranslationConstants = TranslationConstants;

  protected toggleHideCompleted(): void {
    this.filterService.toggleHideCompleted();
  }

  protected onSearchInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.filterService.setSearchTerm(value);
  }

  protected resetFilter(): void {
    this.filterService.setSearchTerm('');
  }
}
