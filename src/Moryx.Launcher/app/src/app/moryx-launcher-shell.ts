import {MoryxShell, SearchRequestCallback, SearchSuggestion} from "@moryx/ngx-web-framework/services";
import {CultureService} from "./services/culture.service";

export class MoryxLauncherShell implements MoryxShell {
  private cultureService: CultureService;

  constructor(cultureService: CultureService) {
    this.cultureService = cultureService;
  }

  initLanguage(): string {
    return this.cultureService.currentCulture().split('-')[0];
  }

  initSearchBar(callback: SearchRequestCallback, disableSearchBox: boolean): void {
  }

  updateSuggestions(suggestions: SearchSuggestion[]): void {
  }
}
