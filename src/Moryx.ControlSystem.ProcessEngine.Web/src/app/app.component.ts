import { Component } from '@angular/core';
import { TranslationConstants } from './extensions/translation-constants.extensions';
import { TranslateService } from '@ngx-translate/core';
import { LanguageService } from '@moryx/ngx-web-framework';
import { JobsComponent } from './components/jobs/jobs.component';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    imports: [
      JobsComponent,
      
    ],
    providers: [
      
    ],
    standalone: true
})
export class AppComponent {
  title = 'Moryx.ControlSystem.ProcessEngine.Web';

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
}
