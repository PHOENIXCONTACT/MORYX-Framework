import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, Inject, QueryList, ViewChildren } from '@angular/core';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';

@Component({
    selector: 'app-confirm-dialog',
    templateUrl: './dialog-confirm.component.html',
    styleUrls: ['./dialog-confirm.component.scss'],
    standalone: true,
    imports: [
      CommonModule,
      MatButtonModule,
      MatDialogModule
    ]
})
export class ConfirmDialogComponent implements AfterViewInit {
  @ViewChildren('actionButton') actionButtons: QueryList<MatButton> | undefined;
  buttons: ConfirmDialogButton[] | undefined;

  constructor(
    public dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogData
  ) {
    this.buttons = data.buttons;
  }

  ngAfterViewInit(): void {
    const focusedButtonIndex = this.buttons?.findIndex(b => b.focused);
    if (!focusedButtonIndex || !this.actionButtons) return;

    const focusedButton = this.actionButtons?.get(focusedButtonIndex!);
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
