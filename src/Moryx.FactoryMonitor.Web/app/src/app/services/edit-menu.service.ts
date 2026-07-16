/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable, signal } from '@angular/core';
import { EditMenuState } from './EditMenutState';

@Injectable({
  providedIn: 'root'
})
export class EditMenuService {

  private readonly _activeState = signal(EditMenuState.Closed);
  readonly activeState = this._activeState.asReadonly();

  public setActiveState(state: EditMenuState) {
    this._activeState.set(state);
  }
}
