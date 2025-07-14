import { CommonModule } from "@angular/common";
import { Component, Inject } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";

@Component({
    selector: "app-attandance-dialog",
    templateUrl: "./availability-dialog.component.html",
    styleUrl: "./availability-dialog.component.scss",
    standalone: true,
    imports: [
      CommonModule,
      MatDialogModule,
      MatFormFieldModule,
      MatInputModule,
      MatDatepickerModule,
      MatButtonModule
    ]
})
export class AvailabilityDialogComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: AttandanceData,
    public dialogRef: MatDialogRef<AvailabilityDialogComponent>
  ) {}

  onSave(){
    this.dialogRef.close(this.data);
  }

  onCancel(){
    this.dialogRef.close(undefined);
  }
}

export interface AttandanceData {}
