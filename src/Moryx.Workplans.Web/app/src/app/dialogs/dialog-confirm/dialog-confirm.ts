/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { AfterViewInit, Component, Inject, viewChildren } from '@angular/core';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';

@Component({
    selector: 'app-confirm-dialog',
    templateUrl: './dialog-confirm.html',
    styleUrls: ['./dialog-confirm.scss'],
    imports: [
    MatButtonModule,
    MatDialogModule
]
})
export class ConfirmDialog implements AfterViewInit {
  actionButtons = viewChildren<MatButton>('actionButton');
  buttons: ConfirmDialogButton[] | undefined;

  constructor(
    public dialogRef: MatDialogRef<ConfirmDialog>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogData
  ) {
    this.buttons = data.buttons;
  }

  ngAfterViewInit(): void {
    const focusedButtonIndex = this.buttons?.findIndex(b => b.focused);
    if (focusedButtonIndex === undefined || focusedButtonIndex < 0) return;

    const focusedButton = this.actionButtons()[focusedButtonIndex];
    if (focusedButton) {
      focusedButton.focus();
    }
  }
}

export interface ConfirmDialogData {
  title: string;
  message: string;
  buttons: ConfirmDialogButton[];
}

export interface ConfirmDialogButton {
  text: string;
  action: Function;
  focused?: boolean;
}

