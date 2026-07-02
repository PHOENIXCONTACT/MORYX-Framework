/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, ElementRef, inject, signal, ViewChild } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatAutocompleteModule, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { SearchService } from '../services/search.service';
import { SearchSuggestion } from '@moryx/ngx-web-framework/services';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '../translation-constants';

@Component({
  selector: 'app-toolbar-search',
  imports: [MatIconModule, MatButtonModule, MatAutocompleteModule, TranslatePipe],
  templateUrl: './toolbar-search.html',
  styleUrl: './toolbar-search.scss',
})
export class ToolbarSearch {
  private searchService = inject(SearchService);

  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;

  protected TranslationConstants = TranslationConstants;

  query = signal('');
  expanded = signal(false);

  suggestions = this.searchService.suggestions;
  hasProvider = this.searchService.hasProvider;
  disableSearchBox = this.searchService.disableSearchBox;

  expand(): void {
    this.expanded.set(true);
    setTimeout(() => this.searchInput?.nativeElement.focus(), 0);
  }

  collapse(): void {
    this.expanded.set(false);
    this.query.set('');
    this.searchInput.nativeElement.value = '';
    this.searchService.search('', false);
    this.searchService.clearSuggestions();
  }

  onQueryChange(value: string): void {
    this.query.set(value);
    if (value.trim()) {
      this.searchService.search(value, false);
    } else {
      this.searchService.clearSuggestions();
    }
  }

  onEnter(): void {
    const q = this.query().trim();
    if (q) {
      this.searchService.search(q, true);
    }
  }

  onOptionSelected(event: MatAutocompleteSelectedEvent): void {
    const suggestion = event.option.value as SearchSuggestion;
    if (suggestion.url) {
      window.location.assign(suggestion.url);
    }
    this.collapse();
  }

  displayFn(suggestion: SearchSuggestion | string): string {
    return typeof suggestion === 'string' ? suggestion : suggestion?.text ?? '';
  }
}
