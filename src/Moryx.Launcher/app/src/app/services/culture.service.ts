/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { computed, inject, Injectable, signal } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { CultureModel } from '../models/culture-model';

@Injectable({
  providedIn: 'root'
})
/** Provides the list of supported cultures and handles culture selection via the ASP.NET culture cookie. */
export class CultureService {
  private cookieService = inject(CookieService);

  private readonly cookieName = '.AspNetCore.Culture';
  private readonly cookieLifetimeDays = 365;

  /** Available cultures provided by the server. */
  private readonly _supportedCultures = signal<CultureModel[]>([]);
  readonly supportedCultures = this._supportedCultures.asReadonly();

  /** Called by the app root to populate the culture list after loading. */
  setSupportedCultures(cultures: CultureModel[]): void {
    this._supportedCultures.set(cultures);
  }

  /** The currently active culture, derived from the ASP.NET culture cookie. */
  currentCulture = computed(() => {
    const cultureCookie = this.cookieService.get(this.cookieName);
    if (!cultureCookie) {
      return '';
    }
    // Cookie format: c=de-DE|uic=de-DE
    const culture = cultureCookie.match(/c=([^|]+)/)?.[1] ?? '';
    return culture;
  });

  constructor() {
    const existing = this.cookieService.get(this.cookieName);
    if (existing) {
      this.cookieService.set(this.cookieName, existing, { expires: this.cookieLifetimeDays, path: '/' });
    }
  }

  selectCulture(culture: CultureModel) {
    const value = `c=${culture.name}|uic=${culture.name}`;
    this.cookieService.set(this.cookieName, value, { expires: this.cookieLifetimeDays, path: '/' });
    window.location.reload();
  }
}
