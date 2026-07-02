/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, DestroyRef, effect, ElementRef, inject, signal, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { SearchService } from '../services/search.service';
import { ShortcutService } from '../services/shortcut.service';
import { SearchSuggestion } from '@moryx/ngx-web-framework/services';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '../translation-constants';

@Component({
  selector: 'app-spotlight-search',
  imports: [FormsModule, MatIconModule, MatButtonModule, MatDividerModule, TranslatePipe],
  templateUrl: './spotlight-search.html',
  styleUrl: './spotlight-search.scss',
  host: {
    '(window:keydown)': 'onKeyDown($event)',
  }
})
export class SpotlightSearch {
  private searchService = inject(SearchService);
  private shortcutService = inject(ShortcutService);
  private destroyRef = inject(DestroyRef);

  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;
  @ViewChild('resultsList') resultsList!: ElementRef<HTMLUListElement>;

  protected TranslationConstants = TranslationConstants;

  isOpen = this.searchService.isOpen;
  query = signal('');
  activeIndex = signal(0);

  suggestions = this.searchService.suggestions;
  hasProvider = this.searchService.hasProvider;
  disableSearchBox = this.searchService.disableSearchBox;

  constructor() {
    this.shortcutService.register(
      {
        key: 'k',
        ctrl: true,
        alt: true,
        label: TranslationConstants.SHORTCUTS.SPOTLIGHT,
        action: () => this.open()
      },
      this.destroyRef
    );

    effect(() => {
      if (this.isOpen()) {
        this.query.set('');
        this.activeIndex.set(0);
        setTimeout(() => this.searchInput?.nativeElement.focus(), 0);
      }
    });
  }

  onKeyDown(event: KeyboardEvent): void {
    if (!this.isOpen()) {
      return;
    }

    switch (event.key) {
      case 'Escape':
        event.preventDefault();
        this.close();
        break;
      case 'ArrowDown':
        event.preventDefault();
        this.activeIndex.update(i => Math.min(i + 1, this.suggestions().length - 1));
        this.scrollActiveIntoView();
        break;
      case 'ArrowUp':
        event.preventDefault();
        this.activeIndex.update(i => Math.max(i - 1, 0));
        this.scrollActiveIntoView();
        break;
      case 'Enter': {
        const q = this.query().trim();
        if (!q) break;
        const active = this.suggestions()[this.activeIndex()];
        if (active) {
          this.navigate(active);
        } else {
          this.searchService.search(q, true);
        }
        break;
      }
    }
  }

  open(): void {
    this.searchService.open();
  }

  close(): void {
    this.searchService.close();
  }

  navigate(suggestion: SearchSuggestion): void {
    if (suggestion.url) {
      window.location.assign(suggestion.url);
    }
    this.close();
  }

  private scrollActiveIntoView(): void {
    const list = this.resultsList?.nativeElement;
    if (!list) return;
    const item = list.children[this.activeIndex()] as HTMLElement | undefined;
    item?.scrollIntoView({block: 'nearest'});
  }

  onQueryChange(value: string): void {
    this.query.set(value);
    this.activeIndex.set(0);
    if (value.trim()) {
      this.searchService.search(value, false);
    } else {
      this.searchService.clearSuggestions();
    }
  }
}
