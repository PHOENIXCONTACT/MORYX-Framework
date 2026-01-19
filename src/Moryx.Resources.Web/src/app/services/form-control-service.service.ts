/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FormControlService {
  public readonly canSave = new BehaviorSubject<boolean>(false);
  constructor() {}

  public onCanSave(state: boolean) {
    this.canSave.next(state);
  }
}

