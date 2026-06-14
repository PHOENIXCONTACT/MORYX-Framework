/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import {Injectable, signal} from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LauncherStateService {

  private readonly stateName = 'LauncherState';

  layout = signal<LauncherLayout>(this.getLayout());

  navCollapsed = signal<boolean>(this.getStoredState()?.navCollapsed ?? false);

  public getLayout(): LauncherLayout {
    const storedState = this.getStoredState();
    if (!storedState) {
      return LauncherLayout.Full;
    }
    if (storedState.fullscreen) {
      return LauncherLayout.Fullscreen;
    }
    if (storedState.operatorMode) {
      return LauncherLayout.Operator;
    }
    return LauncherLayout.Full;
  }

  public updateLayout(layout: LauncherLayout): void {
    this.persistState({
      fullscreen: layout === LauncherLayout.Fullscreen,
      operatorMode: layout === LauncherLayout.Operator,
    });
    this.layout.set(layout);
  }

  public updateNavCollapsed(collapsed: boolean): void {
    this.persistState({ navCollapsed: collapsed });
    this.navCollapsed.set(collapsed);
  }

  private persistState(changes: Partial<LauncherState>): void {
    const currentState = this.getStoredState() ?? { fullscreen: false, operatorMode: false };
    window.localStorage.setItem(this.stateName, JSON.stringify({ ...currentState, ...changes }));
  }

  private getStoredState(): LauncherState | undefined {
    const storedValue = window.localStorage.getItem(this.stateName);
    if (!storedValue) {
      return undefined;
    }
    return JSON.parse(storedValue) as LauncherState;
  }
}

export enum LauncherLayout {
  Full = 'full',
  Operator = 'operator',
  Fullscreen = 'fullscreen',
}

// Kept for localStorage compatibility — not part of the public API
interface LauncherState {
  fullscreen: boolean;
  operatorMode: boolean;
  navCollapsed?: boolean;
}
