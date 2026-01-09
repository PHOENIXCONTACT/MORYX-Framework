import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { LanguageService } from '@moryx/ngx-web-framework';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from './extensions/translation-constants.extensions';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    standalone: true,
    imports:[
      RouterOutlet
    ]
})
export class AppComponent implements OnInit {
  title = 'Orders';
  TranslationConstants = TranslationConstants;

  constructor(
    public translate: TranslateService,
    private languageService: LanguageService
  ) {
    this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translate.setDefaultLang('en');
    this.translate.use(this.languageService.getDefaultLanguage());
  }

  ngOnInit(): void {
    this.translate.get([TranslationConstants.APP.TITLE]).subscribe(title => {
      this.title = title;
    });
  }
}
