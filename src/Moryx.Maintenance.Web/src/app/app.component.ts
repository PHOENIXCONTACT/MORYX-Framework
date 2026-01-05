import { Component, computed, inject, OnInit, signal } from '@angular/core';
import {
  NavigationEnd,
  NavigationStart,
  Router,
  RouterLink,
  RouterOutlet,
} from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MaintenanceStoreService } from './services/maintenance-store.service';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from './extensions/translation-constants.extensions';
import { LanguageService } from '@moryx/ngx-web-framework';

@Component({
  selector: 'app-root',
  imports: [
    CommonModule,
    RouterOutlet,
    MatButtonModule,
    RouterLink,
    MatIconModule,
    MatIconModule,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  providers: [MaintenanceStoreService],
})
export class AppComponent implements OnInit {
  title = 'Moryx.Maintenance.Web';
  storeService = inject(MaintenanceStoreService);

  constructor(
    private router: Router,
    public translate: TranslateService,
    private languageService: LanguageService,
  ) {
    this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
      TranslationConstants.LANGUAGES.ZH,
    ]);
    this.translate.setDefaultLang('en');
    this.translate.use(this.languageService.getDefaultLanguage());
  }

  ngOnInit(): void {}
}
