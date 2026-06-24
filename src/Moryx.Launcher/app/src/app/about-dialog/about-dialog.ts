/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { timer } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { of } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { TranslatePipe } from '@ngx-translate/core';
import { CommonService } from '@api/services/common.service';
import { CultureService } from '../services/culture.service';
import { TranslationConstants } from '../translation-constants';

@Component({
  selector: 'app-about-dialog',
  imports: [MatDialogModule, MatButtonModule, TranslatePipe],
  templateUrl: './about-dialog.html',
  styleUrl: './about-dialog.scss',
})
export class AboutDialog {

  protected TranslationConstants = TranslationConstants;
  isMac = navigator.platform.toUpperCase().includes('MAC');

  shortcuts = [
    { label: TranslationConstants.ABOUT.SHORTCUT_SPOTLIGHT, keys: { mac: '⌘ ⌥ K', other: 'Ctrl+Alt+K' } },
    { label: TranslationConstants.ABOUT.SHORTCUT_FULL_MODE, keys: { mac: '⌘ ⌥ 1', other: 'Ctrl+Alt+1' } },
    { label: TranslationConstants.ABOUT.SHORTCUT_OPERATOR_MODE, keys: { mac: '⌘ ⌥ 2', other: 'Ctrl+Alt+2' } },
    { label: TranslationConstants.ABOUT.SHORTCUT_FULLSCREEN_MODE, keys: { mac: '⌘ ⌥ 3', other: 'Ctrl+Alt+3' } },
  ];

  private commonService = inject(CommonService);
  private cultureService = inject(CultureService);

  private applicationInfo = toSignal(this.commonService.getApplicationInfo()
    .pipe(catchError(() => of(null))));

  private hostInfo = toSignal(this.commonService.getHostInfo()
    .pipe(catchError(() => of(null))));

  private rawServerTime = toSignal(timer(0, 1000)
    .pipe(switchMap(() =>
      this.commonService.getServerTime().pipe(catchError(() => of(null))))
    ));

  serverTime = computed(() => {
    const rawServerTime = this.rawServerTime()?.serverTime;
    if (!rawServerTime) {
      return null;
    }
    return new Intl.DateTimeFormat(this.cultureService.currentCulture() || undefined, {
      dateStyle: 'medium',
      timeStyle: 'medium',
    }).format(new Date(rawServerTime));
  });

  appEntries = computed(() => {
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

  hostEntries = computed(() => {
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
    if (d) parts.push(`${d}d`);
    if (h) parts.push(`${h}h`);
    parts.push(`${m}m`);
    return parts.join(' ');
  }
}
