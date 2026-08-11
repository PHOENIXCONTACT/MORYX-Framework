/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { MoryxShell, SearchRequestCallback, SearchSuggestion } from "@moryx/ngx-web-framework/services";
import { CultureService } from "./services/culture.service";
import { SearchService } from "./services/search.service";

export class MoryxLauncherShell implements MoryxShell {
  private cultureService: CultureService;
  private searchService: SearchService;

  constructor(cultureService: CultureService, searchService: SearchService) {
    this.cultureService = cultureService;
    this.searchService = searchService;
  }

  initLanguage(): string {
    return this.cultureService.currentCulture().split('-')[0];
  }

  initSearchBar(callback: SearchRequestCallback, disableSearchBox: boolean): void {
    this.searchService.register(callback, disableSearchBox);
  }

  updateSuggestions(suggestions: SearchSuggestion[]): void {
    this.searchService.updateSuggestions(suggestions);
  }
}
