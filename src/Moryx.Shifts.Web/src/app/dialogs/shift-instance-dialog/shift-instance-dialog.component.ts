import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import  moment from 'moment';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ShiftInstanceModel } from 'src/app/models/shift-instance-model';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-shift-instance-dialog',
  templateUrl: './shift-instance-dialog.component.html',
  styleUrl: './shift-instance-dialog.component.scss',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    ReactiveFormsModule,
    TranslateModule,
    MatButtonModule
  ]
})
export class ShiftInstanceDialogComponent {
  form = new FormGroup({
    startDate: new FormControl<Date>(new Date()),
    endDate: new FormControl<Date>(new Date()),
  });
  TranslationConstants = TranslationConstants;
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ShiftInstanceModel,
    public dialogRef: MatDialogRef<ShiftInstanceDialogComponent>
  ) {
    this.form.patchValue({
      startDate: data.startDate,
      endDate: data.endDate,
    });
  }

  submit() {
    if (!this.form.valid) return;
    if (this.form.value.startDate)
      this.data.startDate = this.form.value.startDate;
    if (this.form.value.endDate) this.data.endDate = this.form.value.endDate;
    this.dialogRef.close(this.data);
  }

  onStartDateChanged(event: any) {
    const now = moment(this.form.value.startDate);
    const endDate = now.add(this.data.shiftType.duration-1, 'days').toDate();
    this.form.controls.endDate.setValue(endDate);
  }
}
