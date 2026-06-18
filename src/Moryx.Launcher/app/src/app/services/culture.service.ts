/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { computed, Injectable, signal } from '@angular/core';
import { CultureModel } from '../models/culture-model';

@Injectable({
  providedIn: 'root'
})
export class CultureService {
  supportedCultures = signal<CultureModel[]>([]);

  currentCulture = computed(() => {
    const cookies = document.cookie.split(';').map(c => c.trim());
    const rawCookie = cookies.find(c => c.startsWith('.AspNetCore.Culture'));
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
    document.cookie = `.AspNetCore.Culture=${value};path=/;expires=${cookieDate.toUTCString()}`;
    window.location.reload();
  }
}
