/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable, signal } from '@angular/core';
import { SearchRequestCallback, SearchSuggestion } from '@moryx/ngx-web-framework/services';

@Injectable({
  providedIn: 'root'
})
export class SearchService {
  private callback: SearchRequestCallback | null = null;

  /** Whether the spotlight search overlay is currently open. */
  isOpen = signal(false);

  /** Whether a module has registered a search provider. */
  hasProvider = signal(false);

  /** Set by the module when it wants to disable the search box entirely. */
  disableSearchBox = signal(false);

  /** Current suggestions pushed back by the active module. */
  suggestions = signal<SearchSuggestion[]>([]);

  /**
   * Called by a module (via MoryxLauncherShell.initSearchBar) to register
   * itself as the active search provider.
   */
  register(callback: SearchRequestCallback, disableSearchBox: boolean): void {
    this.callback = callback;
    this.disableSearchBox.set(disableSearchBox);
    this.hasProvider.set(true);
    this.suggestions.set([]);
  }

  /** Opens the spotlight search overlay. */
  open(): void {
    this.clearSuggestions();
    this.isOpen.set(true);
  }

  /** Closes the spotlight search overlay and clears suggestions. */
  close(): void {
    this.isOpen.set(false);
    this.clearSuggestions();
  }

  /**
   * Called by the search UI when the user types or submits a query.
   * Forwards the request to the registered module callback.
   */
  search(term: string, complete: boolean): void {
    if (this.callback) {
      this.callback(term, complete);
    }
  }

  /**
   * Called by a module (via MoryxLauncherShell.updateSuggestions) to push
   * search results back to the shell.
   */
  updateSuggestions(suggestions: SearchSuggestion[]): void {
    this.suggestions.set(suggestions);
  }

  /** Clears suggestions, e.g. when the query is emptied. */
  clearSuggestions(): void {
    this.suggestions.set([]);
  }
}
