/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, Inject } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";

@Component({
    selector: "app-attandance-dialog",
    templateUrl: "./availability-dialog.html",
    styleUrl: "./availability-dialog.scss",
    standalone: true,
    imports: [
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatButtonModule
]
})
export class AvailabilityDialog {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: AttandanceData,
    public dialogRef: MatDialogRef<AvailabilityDialog>
  ) {}

  onSave(){
    this.dialogRef.close(this.data);
  }

  onCancel(){
    this.dialogRef.close(undefined);
  }
}

export interface AttandanceData {}

