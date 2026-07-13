/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { DestroyRef, Injectable } from '@angular/core';

export interface ShortcutBinding {
  key: string;
  ctrl?: boolean;
  shift?: boolean;
  alt?: boolean;
  label?: string;
  action: () => void;
}

export interface ShortcutDisplayInfo {
  label: string;
  keys: { mac: string; other: string };
}

@Injectable({
  providedIn: 'root'
})
/** Global keyboard shortcut registry. Bindings are auto-removed when the registering component is destroyed. */
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

  getShortcutInfos(): ShortcutDisplayInfo[] {
    return this.bindings
      .filter(binding => binding.label)
      .map(binding => (
        {
          label: binding.label!,
          keys: this.formatShortcut(binding)
        }));
  }

  private formatShortcut(binding: ShortcutBinding): { mac: string; other: string } {
    const keyName = this.normalizeKeyName(binding.key);

    const macParts: string[] = [];
    if (binding.ctrl) {
      macParts.push('⌘');
    }
    if (binding.alt) {
      macParts.push('⌥');
    }
    if (binding.shift) {
      macParts.push('⇧');
    }
    macParts.push(keyName);

    const otherParts: string[] = [];
    if (binding.ctrl) {
      otherParts.push('Ctrl');
    }
    if (binding.alt) {
      otherParts.push('Alt');
    }
    if (binding.shift) {
      otherParts.push('Shift');
    }
    otherParts.push(keyName);

    return {
      mac: macParts.join(' '),
      other: otherParts.join('+') };
  }

  private normalizeKeyName(key: string): string {
    const digitMatch = key.match(/^Digit(\d)$/i);
    if (digitMatch) {
      return digitMatch[1];
    }
    return key.length === 1 ? key.toUpperCase() : key;
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
