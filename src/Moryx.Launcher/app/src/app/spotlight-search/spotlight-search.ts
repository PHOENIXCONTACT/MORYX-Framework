/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { afterNextRender, Component, DestroyRef, effect, ElementRef, inject, Injector, signal, viewChild } from '@angular/core';
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
  private injector = inject(Injector);

  protected readonly searchInput = viewChild<ElementRef<HTMLInputElement>>('searchInput');
  protected readonly resultsList = viewChild<ElementRef<HTMLUListElement>>('resultsList');

  protected TranslationConstants = TranslationConstants;

  protected isOpen = this.searchService.isSpotlightOpen;
  protected query = signal('');
  protected activeIndex = signal(0);

  protected suggestions = this.searchService.suggestions;
  protected hasProvider = this.searchService.hasProvider;
  protected disableSearchBox = this.searchService.disableSearchBox;

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

        // The input is inside @if(isOpen()), so it doesn't exist in the DOM yet.
        // Wait for Angular to render the view before focusing.
        afterNextRender(() => {
          this.searchInput()?.nativeElement.focus()
        }, { injector: this.injector });
      }
    });
  }

  protected onKeyDown(event: KeyboardEvent): void {
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
        if (!q) {
          break;
        }
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

  protected open(): void {
    this.searchService.open();
  }

  protected close(): void {
    this.query.set('');
    this.searchService.search('', false);
    this.searchService.close();
  }

  protected navigate(suggestion: SearchSuggestion): void {
    if (suggestion.url) {
      window.location.assign(suggestion.url);
    }
    this.close();
  }

  private scrollActiveIntoView(): void {
    const list = this.resultsList()?.nativeElement;
    if (!list) {
      return;
    }
    const item = list.children[this.activeIndex()] as HTMLElement | undefined;
    item?.scrollIntoView({block: 'nearest'});
  }

  protected onQueryChange(value: string): void {
    this.query.set(value);
    this.activeIndex.set(0);
    if (value.trim()) {
      this.searchService.search(value, false);
    } else {
      this.searchService.clearSuggestions();
    }
  }
}
