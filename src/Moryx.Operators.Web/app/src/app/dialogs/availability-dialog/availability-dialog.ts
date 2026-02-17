/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, inject } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";

@Component({
    selector: "app-attandance-dialog",
    templateUrl: "./availability-dialog.html",
    styleUrl: "./availability-dialog.scss",
    imports: [
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatButtonModule
]
})
export class AvailabilityDialog {
  private data = inject<AttandanceData>(MAT_DIALOG_DATA);
  private dialogRef = inject(MatDialogRef<AvailabilityDialog>);

  onSave(){
    this.dialogRef.close(this.data);
  }

  onCancel(){
    this.dialogRef.close(undefined);
  }
}

export interface AttandanceData {}

