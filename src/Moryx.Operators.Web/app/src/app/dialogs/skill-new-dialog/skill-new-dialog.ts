/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { OperatorSkill } from '@app/models/operator-skill-model';
import { AppStoreService } from '@app/services/app-store.service';
import { TranslatePipe } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { provideNativeDateAdapter } from '@angular/material/core';

@Component({
  selector: 'app-skill-new-dialog',
  templateUrl: './skill-new-dialog.html',
  styleUrl: './skill-new-dialog.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatDialogModule,
    MatFormFieldModule,
    MatSelectModule,
    FormsModule,
    ReactiveFormsModule,
    MatDatepickerModule,
    TranslatePipe,
    MatButtonModule,
    MatInputModule
  ],
  providers: [
    provideNativeDateAdapter()
  ],
})
export class SkillNewDialog {
  protected data = inject<OperatorSkill>(MAT_DIALOG_DATA);
  private dialogRef = inject(MatDialogRef<SkillNewDialog>);
  private appStoreService = inject(AppStoreService);

  protected TranslationConstants = TranslationConstants;
  protected skillTypes = this.appStoreService.skillTypes;

  protected save() {
    this.dialogRef.close(this.data);
  }

  protected cancel() {
    this.dialogRef.close();
  }

}

