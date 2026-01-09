import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { NotificationService } from 'src/app/services/notification.service';
import { environment } from 'src/environments/environment';
import { Severity } from '../../api/models';
import { NotificationModel } from '../../api/models/notification-model';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { getIcon } from 'src/app/utils';
import { MarkdownComponent, MarkdownModule, MarkdownService } from 'ngx-markdown';
import { NavigableEntryEditorComponent } from '@moryx/ngx-web-framework';
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector: 'notification-details',
    templateUrl: './notification-details.component.html',
    styleUrls: ['./notification-details.component.scss'],
    imports: [
      CommonModule,
      MatCardModule,
      MatIconModule,
      TranslateModule,
      MarkdownComponent,
      NavigableEntryEditorComponent,
      MatButtonModule
    ],
    standalone: true,
    providers: [MarkdownService]
})
export class NotificationDetailsComponent implements OnInit, OnDestroy {
  notification = signal<NotificationModel | undefined>(undefined);

  subscription: Subscription|undefined;
  TranslationConstants = TranslationConstants;
  getIcon = getIcon;
  environment = environment;

  constructor(
    private notificationService: NotificationService,
    public translate: TranslateService,
  ) {}

  ngOnInit(): void {
    this.subscription = this.notificationService.selection$.subscribe(
      identifier => this.notification.update(_ => this.notificationService.get(identifier))
    );
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  onAcknowledge(notification: NotificationModel): void {
    this.notificationService.acknowledge(notification.identifier);
  }

  isArrayNotEmpty(array: any[] | undefined | null) :boolean{
    return array !== undefined && array !== null && array.length > 0;
  }
}
