/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';

export enum FocusMode {
  Newest = 'newest',
  Stay = 'stay',
  Input = 'input',
}

interface InstructionSettings {
  fullscreen: boolean;
  focusMode: FocusMode;
  instructor: string;
}

@Injectable({
  providedIn: 'root',
})
export class InstructionStateService {
  // TODO: Remove legacy migration (legacyFullscreenKey, cookie migration) in next major version

  private readonly storageKey = 'moryx.visualInstructions.web.settings';
  private readonly cookieName = 'moryx-client-identifier';
  private readonly cookieLifetimeDays = 365;

  private readonly legacyFullscreenKey = 'workerInstructions.fullscreenEnabled';

  private readonly defaultSettings: InstructionSettings = {
    fullscreen: false,
    focusMode: FocusMode.Input,
    instructor: '',
  };

  private settings: InstructionSettings;

  private _fullscreen;
  readonly fullscreen;

  private _focusMode;
  readonly focusMode;

  private _instructor;
  readonly instructor;

  constructor() {
    this.settings = this.load();

    this._fullscreen = signal(this.settings.fullscreen);
    this.fullscreen = this._fullscreen.asReadonly();

    this._focusMode = signal(this.settings.focusMode);
    this.focusMode = this._focusMode.asReadonly();

    this._instructor = signal(this.settings.instructor);
    this.instructor = this._instructor.asReadonly();

    // Sync cookie for SSE endpoint
    if (this.settings.instructor) {
      this.syncCookie(this.settings.instructor);
    }
  }

  private load(): InstructionSettings {
    try {
      const raw = localStorage.getItem(this.storageKey);
      if (raw) {
        const parsed = JSON.parse(raw);
        return {
          fullscreen: typeof parsed.fullscreen === 'boolean' ? parsed.fullscreen : this.defaultSettings.fullscreen,
          focusMode: Object.values(FocusMode).includes(parsed.focusMode) ? parsed.focusMode : this.defaultSettings.focusMode,
          instructor: typeof parsed.instructor === 'string' ? parsed.instructor : this.defaultSettings.instructor,
        };
      }

      // Migrate legacy values
      const migrated = { ...this.defaultSettings };

      const legacyFullscreen = localStorage.getItem(this.legacyFullscreenKey);
      if (legacyFullscreen !== null) {
        localStorage.removeItem(this.legacyFullscreenKey);
        migrated.fullscreen = JSON.parse(legacyFullscreen);
      }

      const legacyCookie = this.readCookie();
      if (legacyCookie) {
        migrated.instructor = legacyCookie;
      }

      return migrated;
    } catch {

    }

    return { ...this.defaultSettings };
  }

  private save(): void {
    try {
      localStorage.setItem(this.storageKey, JSON.stringify(this.settings));
    } catch {}
  }

  toggleFullscreen(): void {
    const newValue = !this.fullscreen();
    this._fullscreen.set(newValue);
    this.settings = { ...this.settings, fullscreen: newValue };
    this.save();
  }

  setFocusMode(mode: FocusMode): void {
    this._focusMode.set(mode);
    this.settings = { ...this.settings, focusMode: mode };
    this.save();
  }

  setInstructor(name: string): void {
    this._instructor.set(name);
    this.settings = { ...this.settings, instructor: name };
    this.save();
    this.syncCookie(name);
  }

  private syncCookie(value: string): void {
    const d = new Date();
    d.setTime(d.getTime() + this.cookieLifetimeDays * 24 * 60 * 60 * 1000);
    const expires = `expires=${d.toUTCString()}`;
    const cpath = '; path=/';
    if (environment.production) {
      document.cookie = `${this.cookieName}=${encodeURI(value)}; ${expires}${cpath}`;
    } else {
      document.cookie = `${this.cookieName}=${encodeURI(value)}; ${expires}${cpath}; samesite=none; secure`;
    }
  }

  private readCookie(): string | undefined {
    const prefix = `${this.cookieName}=`;
    for (const c of document.cookie.split(';')) {
      const trimmed = c.trimStart();
      if (trimmed.startsWith(prefix)) {
        return decodeURI(trimmed.substring(prefix.length));
      }
    }
    return undefined;
  }
}
