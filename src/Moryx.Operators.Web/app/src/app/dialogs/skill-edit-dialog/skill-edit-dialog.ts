/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, Inject, signal } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatSelectModule } from "@angular/material/select";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { OperatorSkill } from "src/app/models/operator-skill-model";
import { SkillType } from "src/app/models/skill-type-model";
import { AppStoreService } from "src/app/services/app-store.service";
import { TranslateModule } from '@ngx-translate/core';
import { MatInputModule } from "@angular/material/input";

@Component({
    selector: "app-skill-edit-dialog",
    templateUrl: "./skill-edit-dialog.html",
    styleUrl: "./skill-edit-dialog.scss",
    standalone: true,
    imports: [
    MatDialogModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    MatButtonModule,
    TranslateModule,
    MatInputModule
]
})
export class SkillEditDialog {
  skillTypes = signal<SkillType[]>([]);

  TranslationConstants = TranslationConstants;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: OperatorSkill,
    public dialogRef: MatDialogRef<SkillEditDialog>,
    private appStoreService: AppStoreService
  ) {
    this.appStoreService.skillTypes$.subscribe(types => this.skillTypes.update(_=> types));
  }

  cancel(){
    this.dialogRef.close();
  }

  save(){
    //TODO: check if data is valid
    this.dialogRef.close(this.data);
  }

  findSkillTypeById(id: number){
    return this.skillTypes().find(x => x.id === id);
  }
}

