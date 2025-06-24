import { Observable } from 'rxjs';
import { Notification } from '../notifications/notifications.component';
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
