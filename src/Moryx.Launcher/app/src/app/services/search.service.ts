/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable, signal } from '@angular/core';
import { SearchRequestCallback, SearchSuggestion } from '@moryx/ngx-web-framework/services';

@Injectable({
  providedIn: 'root'
})
/** Bridges the spotlight search UI with the active module's search provider. */
export class SearchService {
  private callback: SearchRequestCallback | null = null;

  /** Whether the spotlight search overlay is currently open. */
  private readonly _isOpen = signal(false);
  readonly isOpen = this._isOpen.asReadonly();

  /** Whether a module has registered a search provider. */
  private readonly _hasProvider = signal(false);
  readonly hasProvider = this._hasProvider.asReadonly();

  /** Set by the module when it wants to disable the search box entirely. */
  private readonly _disableSearchBox = signal(false);
  readonly disableSearchBox = this._disableSearchBox.asReadonly();

  /** Current suggestions pushed back by the active module. */
  private readonly _suggestions = signal<SearchSuggestion[]>([]);
  readonly suggestions = this._suggestions.asReadonly();

  /**
   * Called by a module (via MoryxLauncherShell.initSearchBar) to register
   * itself as the active search provider.
   */
  register(callback: SearchRequestCallback, disableSearchBox: boolean): void {
    this.callback = callback;
    this._disableSearchBox.set(disableSearchBox);
    this._hasProvider.set(true);
    this._suggestions.set([]);
  }

  /** Opens the spotlight search overlay. */
  open(): void {
    this.clearSuggestions();
    this._isOpen.set(true);
  }

  /** Closes the spotlight search overlay and clears suggestions. */
  close(): void {
    this._isOpen.set(false);
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
    this._suggestions.set(suggestions);
  }

  /** Clears suggestions, e.g. when the query is emptied. */
  clearSuggestions(): void {
    this._suggestions.set([]);
  }
}
