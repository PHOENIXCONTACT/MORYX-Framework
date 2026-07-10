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

  public activeState = signal(EditMenuState.Closed);

  public setActiveState(state: EditMenuState) {
    this.activeState.set(state);
  }
}
