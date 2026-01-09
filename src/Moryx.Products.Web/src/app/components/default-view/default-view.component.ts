import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { EmptyStateComponent } from '@moryx/ngx-web-framework';
import { TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
@Component({
    selector: 'app-default-view',
    templateUrl: './default-view.component.html',
    styleUrls: ['./default-view.component.scss'],
    imports: [
      CommonModule,
      EmptyStateComponent
    ],
    standalone: true
})
export class DefaultViewComponent implements OnInit {
  headerText = signal('');
  messageText = signal('');

  TranslationConstants = TranslationConstants;

  constructor(private router: Router, public translate: TranslateService) {}

  ngOnInit(): void {
    this.getHeaderAndMessage();
  }

  getHeaderAndMessage() {
    this.translate
      .get([
        TranslationConstants.APP.EMPTY_STATE_HEADER,
        TranslationConstants.APP.EMPTY_STATE_TEXT,
        TranslationConstants.APP.EMPTY_STATE_RECIPES_HEADER,
        TranslationConstants.APP.EMPTY_STATE_RECIPES_TEXT,
        TranslationConstants.APP.EMPTY_STATE_PARTS_HEADER,
        TranslationConstants.APP.EMPTY_STATE_PARTS_TEXT,
      ])
      .subscribe(translations => {
        if (this.router.url.includes('recipes')) {
          this.headerText.update(_=> translations[TranslationConstants.APP.EMPTY_STATE_RECIPES_HEADER]);
          this.messageText.update(_=> translations[TranslationConstants.APP.EMPTY_STATE_RECIPES_TEXT]);
          return;
        }

        if (this.router.url.includes('parts')) {
          this.headerText.update(_=> translations[TranslationConstants.APP.EMPTY_STATE_PARTS_HEADER]);
          this.messageText.update(_=> translations[TranslationConstants.APP.EMPTY_STATE_PARTS_TEXT]);
          return;
        }

        this.headerText.update(_=> translations[TranslationConstants.APP.EMPTY_STATE_HEADER]);
        this.messageText.update(_=> translations[TranslationConstants.APP.EMPTY_STATE_TEXT]);
      });
  }
}
