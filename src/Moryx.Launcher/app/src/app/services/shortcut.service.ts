/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { DestroyRef, inject, Injectable } from '@angular/core';

export interface ShortcutBinding {
  key: string;
  ctrl?: boolean;
  shift?: boolean;
  alt?: boolean;
  action: () => void;
}

@Injectable({
  providedIn: 'root'
})
export class ShortcutService {
  private bindings: ShortcutBinding[] = [];

  constructor() {
    window.addEventListener('keydown', (event) => this.onKeyDown(event));
  }

  register(binding: ShortcutBinding, destroyRef: DestroyRef): void {
    this.bindings.push(binding);
    destroyRef.onDestroy(() => {
      const index = this.bindings.indexOf(binding);
      if (index >= 0) {
        this.bindings.splice(index, 1);
      }
    });
  }

  private onKeyDown(event: KeyboardEvent): void {
    const ctrl = event.metaKey || event.ctrlKey;

    for (const binding of this.bindings) {
      const key = binding.key.toLowerCase();

      // The key to match — checked against both event.key and event.code (e.g. 'k', 'Digit1').
      const keyMatch = event.key.toLowerCase() === key
        || event.code.toLowerCase() === key
        || event.code.toLowerCase() === `key${key}`;

      if (keyMatch && ctrl === !!binding.ctrl && event.shiftKey === !!binding.shift
        && event.altKey === !!binding.alt) {
        event.preventDefault();
        binding.action();
        return;
      }
    }
  }
}
