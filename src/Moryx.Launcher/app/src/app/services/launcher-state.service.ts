/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import {Injectable, signal} from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LauncherStateService {

  stateName = "LauncherState";

  state = signal<LauncherState>(this.getState() ?? { fullscreen: false, operatorMode: false });

  public getState(): LauncherState | undefined {
    const value = window.localStorage.getItem(this.stateName);
    if (!value) return undefined;
    return JSON.parse(value) as LauncherState;
  }

  public updateState(value: LauncherState): void {
    window.localStorage.setItem(this.stateName, JSON.stringify(value));
    this.state.set(value);
  }

}

export interface LauncherState{
  fullscreen: boolean;
  operatorMode: boolean;
}
