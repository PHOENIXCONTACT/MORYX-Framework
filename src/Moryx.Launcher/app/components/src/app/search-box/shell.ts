/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Observable } from 'rxjs';

export type Severity = 'Info' | 'Warning' | 'Error' | 'Fatal';

export interface Notification {
  severity: Severity;
  title: string;
}

export interface MoryxShell extends Object {
  initSearchBar(
    callback: SearchRequestCallback,
    disableSearchBox: boolean
  ): void;
  updateSuggestions(suggestions: SearchSuggestion[]): void;
  initLanguage(): string;
  notifications: Observable<Array<Notification>>;
}

export interface SearchSuggestion {
  text: string;
  url?: string;
}
export interface SearchRequest {
  term: string;
  submitted: boolean;
}

export interface SearchSuggestion {
  text: string;
  url?: string;
}

export interface SearchRequestCallback {
  (term: string, complete: boolean): void;
}
export interface NotificationStreamCallback {
  (values: Array<Notification>): void;
}

