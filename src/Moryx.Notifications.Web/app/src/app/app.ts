/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnDestroy, OnInit, signal } from "@angular/core";
import { LanguageService } from "@moryx/ngx-web-framework/services";
import { EmptyState } from "@moryx/ngx-web-framework/empty-state";
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
import { Notifications } from "./components/notifications/notifications";
import { NotificationDetails } from "./components/notification-details/notification-details";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";

@Component({
  selector: "app-root",
  templateUrl: "./app.html",
  styleUrls: ["./app.scss"],
  imports: [
    CommonModule,
    MatSidenavModule,
    MatToolbarModule,
    Notifications,
    NotificationDetails,
    TranslateModule,
    MatProgressSpinnerModule,
    EmptyState
  ],
  standalone: true,
})
export class App implements OnInit, OnDestroy {
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
    this.translate.setFallbackLang("en");
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

