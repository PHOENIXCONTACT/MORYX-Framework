/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { effect, Injectable, signal } from '@angular/core';

export type ThemeMode = 'light' | 'dark' | 'system';

@Injectable({
  providedIn: 'root'
})
/** Manages the light/dark/system theme and applies it to the document root. */
export class ThemeService {
  private readonly storageKey = 'LauncherTheme';
  private systemDarkQuery = window.matchMedia('(prefers-color-scheme: dark)');
  private systemDark = signal(this.systemDarkQuery.matches);

  /** The active theme mode (light, dark, system). Persisted. */
  private readonly _mode = signal<ThemeMode>(this.getStoredMode());
  readonly mode = this._mode.asReadonly();

  constructor() {
    this.systemDarkQuery.addEventListener('change', (e) => this.systemDark.set(e.matches));

    effect(() => {
      const mode = this._mode();
      const isDark = mode === 'dark' || (mode === 'system' && this.systemDark());
      document.documentElement.classList.toggle('dark-theme', isDark);
      window.localStorage.setItem(this.storageKey, mode);
    });
  }

  setMode(mode: ThemeMode): void {
    this._mode.set(mode);
  }

  private getStoredMode(): ThemeMode {
    const stored = window.localStorage.getItem(this.storageKey);
    if (stored === 'light' || stored === 'dark' || stored === 'system') {
      return stored;
    }
    return 'light'; //TODO change to system in the next major
  }
}
