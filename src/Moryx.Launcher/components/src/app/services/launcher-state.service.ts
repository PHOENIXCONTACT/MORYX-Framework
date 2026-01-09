import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LauncherStateService {

  stateName = "LauncherState";
  constructor() { }

  public getState(): LauncherState | undefined{
    const value =  window.localStorage.getItem(this.stateName);
    if(!value) return undefined;
    return <LauncherState>JSON.parse(value);
  }

  public updateState(value: LauncherState): void{
    window.localStorage.setItem(this.stateName,JSON.stringify(value));
  }

}

export interface LauncherState{
  fullscreen: boolean;
  operatorMode: boolean;
}