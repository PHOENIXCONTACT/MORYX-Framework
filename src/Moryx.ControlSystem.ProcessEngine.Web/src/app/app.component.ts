/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, signal } from "@angular/core";
import { TranslationConstants } from "./extensions/translation-constants.extensions";
import { TranslateService } from "@ngx-translate/core";
import { LanguageService } from "@moryx/ngx-web-framework";
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
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  imports: [RouterOutlet, MatIconModule, MatButtonModule, RouterLink, RouterLinkActive],
  providers: [],
  standalone: true,
})
export class AppComponent {
  title = "Moryx.ControlSystem.ProcessEngine.Web";
  header = signal("Processes");

  constructor(
    public translate: TranslateService,
    private languageService: LanguageService,
    private router: Router
  ) {
    this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translate.setDefaultLang("en");
    this.translate.use(this.languageService.getDefaultLanguage());
    this.router.events.subscribe((event) => {
      event instanceof NavigationEnd ? this.updateTitle(event) : null;
    });
  }

  private async getTranslations(): Promise<{ [key: string]: string }> {
    return await this.translate
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

