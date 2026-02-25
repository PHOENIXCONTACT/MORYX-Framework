import { Component, inject, signal } from "@angular/core";
import { TranslationConstants } from "./extensions/translation-constants.extensions";
import { TranslateService } from "@ngx-translate/core";
import { LanguageService } from "@moryx/ngx-web-framework/services";
import {
  NavigationEnd,
  Router,
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
  providers: []
})
export class App {
  private translateService = inject(TranslateService);
  private languageService = inject(LanguageService);
  private router = inject(Router);

  title = "Moryx.ControlSystem.ProcessEngine.Web";
  header = signal("Processes");

  constructor() {
    this.translateService.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translateService.setFallbackLang("en");
    this.translateService.use(this.languageService.getDefaultLanguage());
    this.router.events.subscribe((event) => {
      event instanceof NavigationEnd ? this.updateTitle(event) : null;
    });
  }

  private async getTranslations(): Promise<{ [key: string]: string }> {
    return await this.translateService
      .get([TranslationConstants.PROCESS_HOLDER_GROUPS.PROCESS_HOLDER_GROUP_TITLE,
        TranslationConstants.PROCESS_HOLDER_GROUPS.PROCESSES_TITLE,
      ])
      .toAsync();
  }

  updateTitle(event: NavigationEnd) {
    switch (event.url) {
      case "/jobs":
        this.getTranslations().then(
          values =>
            this.header.set(values[TranslationConstants.PROCESS_HOLDER_GROUPS.PROCESSES_TITLE])
        )
        break;
      case "/process-holders":
       this.getTranslations().then(
          values =>
            this.header.set(values[TranslationConstants.PROCESS_HOLDER_GROUPS.PROCESS_HOLDER_GROUP_TITLE])
        )
        break;
      default:
        break;
    }
  }

}

