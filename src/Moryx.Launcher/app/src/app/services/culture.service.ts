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
export class CultureService {
  private cookieService = inject(CookieService);

  supportedCultures = signal<CultureModel[]>([]);

  currentCulture = computed(() => {
    const rawCookie = this.cookieService.get('.AspNetCore.Culture');
    if (!rawCookie) {
      return '';
    }
    const decoded = decodeURIComponent(rawCookie);
    return decoded.split(/=|\|/)[2];
  });

  selectCulture(culture: CultureModel) {
    const cookieDate = new Date;
    cookieDate.setFullYear(cookieDate.getFullYear() + 1);
    const value = encodeURIComponent(`c=${culture.name}|uic=${culture.name}`);
    this.cookieService.set('.AspNetCore.Culture', value, { expires: cookieDate, path: '/' });
    window.location.reload();
  }
}
