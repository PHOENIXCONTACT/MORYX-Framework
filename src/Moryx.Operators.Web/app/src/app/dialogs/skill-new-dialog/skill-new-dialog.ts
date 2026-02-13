/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, Inject, OnInit, signal } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { OperatorSkill } from 'src/app/models/operator-skill-model';
import { SkillType } from 'src/app/models/skill-type-model';
import { AppStoreService } from 'src/app/services/app-store.service';
import { TranslateModule } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';

@Component({
    selector: 'app-skill-new-dialog',
    templateUrl: './skill-new-dialog.html',
    styleUrl: './skill-new-dialog.scss',
    standalone: true,
    imports: [
    MatDialogModule,
    MatFormFieldModule,
    MatSelectModule,
    FormsModule,
    ReactiveFormsModule,
    MatDatepickerModule,
    TranslateModule,
    MatButtonModule,
    MatInputModule
]
})
export class SkillNewDialog implements OnInit {
  TranslationConstants = TranslationConstants;
  skillTypes = signal<SkillType[]>([]);

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: OperatorSkill ,
    public dialogRef: MatDialogRef<SkillNewDialog>,
    private appStoreService: AppStoreService
  ){
  }


  ngOnInit(): void {
    this.appStoreService.skillTypes$.subscribe(types => {
      this.skillTypes.update(_=> types);
    })
  }

  save(){
    this.dialogRef.close(this.data);
  }

  cancel(){
    this.dialogRef.close();
  }

}

