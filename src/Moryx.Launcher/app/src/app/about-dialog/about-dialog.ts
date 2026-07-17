/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { CommonService } from '@api/services/common.service';
import { ApplicationInformationResponse, HostInformationResponse, ServerTimeResponse } from '@api/models';
import { CultureService } from '../services/culture.service';
import { ShortcutService } from '../services/shortcut.service';
import { TranslationConstants } from '../translation-constants';

@Component({
  selector: 'app-about-dialog',
  imports: [MatDialogModule, MatButtonModule, TranslatePipe],
  templateUrl: './about-dialog.html',
  styleUrl: './about-dialog.scss',
})
export class AboutDialog implements OnInit {
  private commonService = inject(CommonService);
  private cultureService = inject(CultureService);
  private shortcutService = inject(ShortcutService);
  private destroyRef = inject(DestroyRef);

  protected TranslationConstants = TranslationConstants;
  protected isMac = navigator.platform.toUpperCase().includes('MAC');

  protected shortcuts = this.shortcutService.getShortcutInfos();

  private applicationInfo = signal<ApplicationInformationResponse | null>(null);
  private hostInfo = signal<HostInformationResponse | null>(null);
  private rawServerTime = signal<ServerTimeResponse | null>(null);

  ngOnInit() {
    this.commonService.getApplicationInfo().then(v => this.applicationInfo.set(v)).catch(() => {});
    this.commonService.getHostInfo().then(v => this.hostInfo.set(v)).catch(() => {});

    this.fetchServerTime();
    const interval = setInterval(() => this.fetchServerTime(), 1000);
    this.destroyRef.onDestroy(() => clearInterval(interval));
  }

  private fetchServerTime() {
    this.commonService.getServerTime().then(v => this.rawServerTime.set(v)).catch(() => {});
  }

  protected serverTime = computed(() => {
    const rawServerTime = this.rawServerTime()?.serverTime;
    if (!rawServerTime) {
      return null;
    }
    return new Intl.DateTimeFormat(this.cultureService.currentCulture() || undefined, {
      dateStyle: 'medium',
      timeStyle: 'medium',
    }).format(new Date(rawServerTime));
  });

  protected appEntries = computed(() => {
    const applicationInfo = this.applicationInfo();
    if (!applicationInfo) {
      return [];
    }
    return [
      {label: TranslationConstants.ABOUT.LABEL_TITLE, value: applicationInfo.assemblyTitle},
      {label: TranslationConstants.ABOUT.LABEL_PRODUCT, value: applicationInfo.assemblyProduct},
      {label: TranslationConstants.ABOUT.LABEL_DESCRIPTION, value: applicationInfo.assemblyDescription},
      {label: TranslationConstants.ABOUT.LABEL_VERSION, value: applicationInfo.assemblyVersion},
      {
        label: TranslationConstants.ABOUT.LABEL_INFORMATIONAL_VERSION,
        value: applicationInfo.assemblyInformationalVersion
      },
      {label: TranslationConstants.ABOUT.LABEL_COMPANY, value: applicationInfo.assemblyCompanyName},
      {label: TranslationConstants.ABOUT.LABEL_CONFIGURATION, value: applicationInfo.assemblyConfiguration},
      {label: TranslationConstants.ABOUT.LABEL_COPYRIGHT, value: applicationInfo.assemblyCopyright},
      {label: TranslationConstants.ABOUT.LABEL_TARGET_FRAMEWORK, value: applicationInfo.targetFramework},
    ].filter(e => e.value);
  });

  protected hostEntries = computed(() => {
    const hostInfo = this.hostInfo();
    if (!hostInfo) {
      return [];
    }
    return [
      {label: TranslationConstants.ABOUT.LABEL_MACHINE_NAME, value: hostInfo.machineName},
      {label: TranslationConstants.ABOUT.LABEL_OS, value: hostInfo.osInformation},
      {
        label: TranslationConstants.ABOUT.LABEL_UPTIME,
        value: hostInfo.upTime != null ? this.formatUptime(hostInfo.upTime) : null
      },
    ].filter(e => e.value);
  });

  private formatUptime(milliseconds: number): string {
    const seconds = milliseconds / 1000;
    const d = Math.floor(seconds / 86400);
    const h = Math.floor((seconds % 86400) / 3600);
    const m = Math.floor((seconds % 3600) / 60);
    const parts = [];
    if (d) {
      parts.push(`${d}d`);
    }
    if (h) {
      parts.push(`${h}h`);
    }
    parts.push(`${m}m`);
    return parts.join(' ');
  }
}
