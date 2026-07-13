/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
/** Manages the launcher's layout state (layout, nav, regions) and persists it to localStorage. */
export class LauncherLayoutService {

  private readonly stateName = 'LauncherState'; // TODO: Rename to LauncherLayout in the next major

  /** The active layout mode (full, operator, fullscreen). Persisted. */
  layout = signal<LauncherLayout>(this.getLayout());

  /** Whether the side navigation is collapsed. Persisted. */
  navCollapsed = signal<boolean>(this.getStoredState()?.navCollapsed ?? false);

  /** Whether a top region slot has content projected. Set by layout components. */
  topRegionAvailable = signal(false);

  /** Whether the user has enabled the top region. Persisted. */
  topRegionEnabled = signal<boolean>(this.getStoredState()?.topRegionEnabled ?? true);

  /** Whether a right region slot has content projected. Set by layout components. */
  rightRegionAvailable = signal(false);

  /** Whether the user has enabled the right region. Persisted. */
  rightRegionEnabled = signal<boolean>(this.getStoredState()?.rightRegionEnabled ?? true);

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
    this.persistState({navCollapsed: collapsed});
    this.navCollapsed.set(collapsed);
  }

  public updateTopRegionEnabled(enabled: boolean): void {
    this.persistState({topRegionEnabled: enabled});
    this.topRegionEnabled.set(enabled);
  }

  public updateRightRegionEnabled(enabled: boolean): void {
    this.persistState({rightRegionEnabled: enabled});
    this.rightRegionEnabled.set(enabled);
  }

  private persistState(changes: Partial<LauncherState>): void {
    const currentState = this.getStoredState() ?? {fullscreen: false, operatorMode: false};
    window.localStorage.setItem(this.stateName, JSON.stringify({...currentState, ...changes}));
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


interface LauncherState {
  // Kept for localStorage compatibility
  // TODO use enum in the next major
  fullscreen: boolean;
  operatorMode: boolean;

  navCollapsed?: boolean;
  topRegionEnabled?: boolean;
  rightRegionEnabled?: boolean;
}
