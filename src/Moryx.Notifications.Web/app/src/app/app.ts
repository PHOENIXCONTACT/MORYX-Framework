/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, inject, OnDestroy, OnInit, signal, ChangeDetectionStrategy } from "@angular/core";
import { EmptyState } from "@moryx/ngx-web-framework/empty-state";
import { TranslatePipe } from "@ngx-translate/core";
import { environment } from "../environments/environment";
import { TranslationConstants } from "./translation-constants";
import { NotificationService } from "./services/notification.service";

import ConnectionState from "./models/ConnectionState";
import "./extensions/notification.extensions";

import { MatSidenavModule } from "@angular/material/sidenav";
import { MatToolbarModule } from "@angular/material/toolbar";
import { Notifications } from "./components/notifications/notifications";
import { NotificationDetails } from "./components/notification-details/notification-details";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";

@Component({
  selector: "app-root",
  templateUrl: "./app.html",
  styleUrls: ["./app.scss"],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatSidenavModule,
    MatToolbarModule,
    Notifications,
    NotificationDetails,
    TranslatePipe,
    MatProgressSpinnerModule,
    EmptyState
  ],
  host: {
    '(window:beforeunload)': 'disconnectEvents()'
  }
})
export class App implements OnInit, OnDestroy {
  private notificationService = inject(NotificationService);

  protected isLoading = computed(() => this.notificationService.state() !== ConnectionState.Connected);
  protected isEmpty = computed(() => !this.notificationService.notifications().length);
  protected notificationsToolbarImage = signal(
    environment.assets + "assets/notifications_toolbar.jpg");

  protected TranslationConstants = TranslationConstants;

  ngOnInit(): void {
    this.notificationService.connect();
  }

  ngOnDestroy(): void {
    this.disconnectEvents();
  }

  protected disconnectEvents(): void {
    this.notificationService.disconnect();
  }
}

