import { Component, inject, ChangeDetectionStrategy } from "@angular/core";
import { TranslationConstants } from "./extensions/translation-constants.extensions";
import { TranslateService } from "@ngx-translate/core";
import { LanguageService } from "@moryx/ngx-web-framework/services";
import {
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from "@angular/router";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";

@Component({
  selector: "app-root",
  templateUrl: "./app.html",
  styleUrls: ["./app.scss"],
  imports: [RouterOutlet, MatIconModule, MatButtonModule, RouterLink, RouterLinkActive],
  changeDetection: ChangeDetectionStrategy.Eager,
  providers: []
})
export class App {
  private translateService = inject(TranslateService);
  private languageService = inject(LanguageService);

  constructor() {
    this.translateService.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translateService.setFallbackLang("en");
    this.translateService.use(this.languageService.getFallbackLang());
  }
}

