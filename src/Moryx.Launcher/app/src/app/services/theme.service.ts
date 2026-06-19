/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable, signal } from '@angular/core';

export type ThemeMode = 'light' | 'dark' | 'system';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly storageKey = 'LauncherTheme';
  private systemDarkQuery = window.matchMedia('(prefers-color-scheme: dark)');

  mode = signal<ThemeMode>(this.getStoredMode());

  constructor() {
    this.systemDarkQuery.addEventListener('change', () => this.applyTheme());
    this.applyTheme();
  }

  setMode(mode: ThemeMode): void {
    this.mode.set(mode);
    window.localStorage.setItem(this.storageKey, mode);
    this.applyTheme();
  }

  private getStoredMode(): ThemeMode {
    const stored = window.localStorage.getItem(this.storageKey);
    if (stored === 'light' || stored === 'dark' || stored === 'system') {
      return stored;
    }
    return 'system';
  }

  private applyTheme(): void {
    const mode = this.mode();
    const isDark = mode === 'dark' || (mode === 'system' && this.systemDarkQuery.matches);
    document.documentElement.classList.toggle('dark-theme', isDark);
  }
}
