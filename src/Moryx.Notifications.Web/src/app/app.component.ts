import { Component, OnDestroy, OnInit, signal } from "@angular/core";
import { EmptyStateComponent, LanguageService } from "@moryx/ngx-web-framework";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { environment } from "src/environments/environment";
import { TranslationConstants } from "./extensions/translation-constants.extensions";
import { NotificationService } from "./services/notification.service";
import ConnectionState from "./models/ConnectionState";
import "./extensions/notification.extensions";
import { Subscription } from "rxjs";
import { CommonModule } from "@angular/common";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatToolbarModule } from "@angular/material/toolbar";
import { NotificationsComponent } from "./components/notifications/notifications.component";
import { NotificationDetailsComponent } from "./components/notification-details/notification-details.component";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  imports: [
    CommonModule,
    MatSidenavModule,
    MatToolbarModule,
    NotificationsComponent,
    NotificationDetailsComponent,
    TranslateModule,
    MatProgressSpinnerModule,
    EmptyStateComponent
  ],
  standalone: true,
})
export class AppComponent implements OnInit, OnDestroy {
  isLoading = signal(true);
  isEmpty = signal(true);
  notificationsToolbarImage = signal(
    environment.assets + "assets/notifications_toolbar.jpg");


  title = "Moryx.Notifications.Web";
  TranslationConstants = TranslationConstants;
  private stateSubscription: Subscription | undefined;
  private notificationSubscription: Subscription | undefined;

  constructor(
    private languageService: LanguageService,
    public translate: TranslateService,
    private notificationService: NotificationService
  ) {
    this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
    this.translate.setDefaultLang("en");
    this.translate.use(this.languageService.getDefaultLanguage());
  }

  ngOnInit(): void {
    this.stateSubscription = this.notificationService.state$.subscribe(
      (state) => {
        if (state == ConnectionState.Connected) this.isLoading.update(_ => false);
      }
    );
    this.notificationSubscription =
      this.notificationService.notifications$.subscribe((n) => {
        this.isEmpty.update(_ =>  !n.length);
      });
  }

  ngOnDestroy(): void {
    this.stateSubscription?.unsubscribe();
    this.notificationSubscription?.unsubscribe();
  }
}
