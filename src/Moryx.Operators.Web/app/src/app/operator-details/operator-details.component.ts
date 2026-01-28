/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, input, OnInit, signal, untracked } from "@angular/core";
import { OperatorModel } from "../api/models/operator-model";
import { OperatorManagementService } from "../api/services";
import { ActivatedRoute, RouterLink } from "@angular/router";
import { TranslationConstants } from "../extensions/translation-constants.extensions";
import { OperatorSkillView } from "../models/type";
import { OperatorSkill } from "../models/operator-skill-model";
import { SKILLS } from "../models/dummy-data";
import { dateToString } from "../models/utils";
import { MatTableDataSource, MatTableModule } from "@angular/material/table";
import { MatDialog } from "@angular/material/dialog";
import { SkillNewDialogComponent } from "../dialogs/skill-new-dialog/skill-new-dialog.component";
import { SkillEditDialogComponent } from "../dialogs/skill-edit-dialog/skill-edit-dialog.component";
import { ConfirmationDialogComponent } from "../dialogs/confirmation-dialog/confirmation-dialog.component";
import { OperatorViewModel } from "../models/operator-view-model";
import { AssignableOperator } from "../api/models/assignable-operator";
import { skillToOperatorSkill, skillTypeToModel } from "../models/model-converter";
import { SkillTypeModel } from "../api/models/skill-type-model";
import { timeInterval } from "rxjs";
import { AppStoreService } from "../services/app-store.service";
import { IOperatorAssignable } from "../api/models/i-operator-assignable";
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { MatIconModule } from "@angular/material/icon";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { FormsModule } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";

@Component({
    selector: "app-operator-details",
    templateUrl: "./operator-details.component.html",
    styleUrl: "./operator-details.component.scss",
    standalone: true,
    imports: [
    MatIconModule,
    MatSidenavModule,
    MatTooltipModule,
    TranslateModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    RouterLink
]
})
export class OperatorDetailsComponent implements OnInit {
  id = input.required<string>();
  editMode =  signal(false);
  operator = signal<AssignableOperator>({
    assignedResources: [],
    firstName: '',
    identifier: '',
    lastName: '',
    pseudonym: '',
    signedIn: false
  });
  skillView = signal<OperatorSkillView>('Current');
  skillTypes = signal<SkillTypeModel[]>([]);
  operatorViewModel = signal<OperatorViewModel | undefined>(undefined);

  TranslationConstants = TranslationConstants;
  dateToString = dateToString;
  dataSource!: MatTableDataSource<OperatorSkill>;
  skillToOperatorSkill = skillToOperatorSkill;
  skillTypeToModel = skillTypeToModel;
  displayedColumns: string[] = ['type','obtainedOn','expiresOn','actions'];

  constructor(
    private appStoreService: AppStoreService,
    private activatedRoute: ActivatedRoute,
    private dialog: MatDialog,
    public translate: TranslateService,
  ) {
    effect(() => {
      const id = this.id();
     untracked(() => this.initialize(id))
    })
  }

  ngOnInit(): void {
  }

  initialize(id: string){
    const identifier = id;
    if(!identifier) return;
    const operatorDataPromise = this.appStoreService.getOperator(identifier);

    operatorDataPromise.then(result => {
      if(!result) return;

      this.operatorViewModel.update(_=> result);
      this.operator.update(_=> result.data);
    });

    
   this.loadSkills();

    this.appStoreService
      .skillTypes$
      .subscribe(types => this.skillTypes.update(_=> types.map(skillTypeToModel)));
  }

  loadSkills(){
    this.appStoreService.getSkillFromRemoteSource()
    .subscribe(skills =>{
      const skillModels = skills.filter(e => e.operatorIdentifier === this.operator().identifier).map(skillToOperatorSkill);
      this.dataSource = new MatTableDataSource(skillModels);
    });
  }

  onStopEditing() {
    this.operator.update(_=> this.appStoreService.cancelEditing(this.operator()));
    this.editMode.update(_=> false);
  }

  onStartEditing() {
    this.editMode.update(_=> true);
  }

  async onSave() {
    await this.appStoreService.updateOperator(this.operator())
    .then(() => {
      this.editMode.update(_=> false);
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  onAddSkillClick(){
    const dialogResult = this.dialog.open(SkillNewDialogComponent,{
      width: '400px',
      data: <OperatorSkill>{
        operatorId: this.operator().identifier
      }
    });

    dialogResult.afterClosed().subscribe(result =>{
      if(!result) result;

      this.appStoreService.addSkill(this.operatorViewModel()!,result);
      setTimeout(() => this.loadSkills(),500);
    });
  }


  async onDeleteSkillClick(skill: OperatorSkill){
    const translations = await this.translate
          .get([
            TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_TITLE,
            TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_MESSAGE
          ])
          .toAsync();
    const dialogRef = this.dialog.open(ConfirmationDialogComponent,{
      data:{
        dialogMessage: translations[TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_MESSAGE],
        dialogTitle: translations[TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_TITLE],
        dialogResult: 'NO'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if(result.dialogResult === 'NO') return;

      this.appStoreService.deleteSkill(skill);
      setTimeout(() => this.loadSkills(),500);
    });
  }

  findSkillTypeById(id: number){
    return this.skillTypes().find(x => x.id === id);
  }
}

