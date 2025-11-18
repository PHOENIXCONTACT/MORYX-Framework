import { Component, OnInit, signal } from "@angular/core";
import { OperatorViewModel } from "../models/operator-view-model";
import { MatDialog } from "@angular/material/dialog";
import { ConfirmationDialogComponent } from "../dialogs/confirmation-dialog/confirmation-dialog.component";
import { AddOperatorComponentDialog } from "../dialogs/add-operator/add-operator.component";
import { TranslationConstants } from "../extensions/translation-constants.extensions";
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { OperatorSkill } from "../models/operator-skill-model";
import { SKILLS } from "../models/dummy-data";
import { SkillTypeModel } from "../api/models/skill-type-model";
import { SkillType } from "../models/skill-type-model";
import { skillToOperatorSkill, skillTypeToModel } from "../models/model-converter";
import { Router, RouterLink } from "@angular/router";
import { AppStoreService } from "../services/app-store.service";
import { CommonModule } from "@angular/common";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatIconModule } from "@angular/material/icon";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatExpansionModule } from "@angular/material/expansion";
import { OperatorSkillChipsComponent } from "../operator-skill-chips/operator-skill-chips.component";
import { MatButtonModule } from "@angular/material/button";

@Component({
    selector: "app-operators-management",
    templateUrl: "./operators-management.component.html",
    styleUrl: "./operators-management.component.scss",
    standalone: true,
    imports: [
      CommonModule,
      MatTooltipModule,
      MatIconModule,
      MatSidenavModule,
      MatExpansionModule,
      OperatorSkillChipsComponent,
      RouterLink,
      TranslateModule,
      MatButtonModule
    ]
})
export class OperatorsManagementComponent implements OnInit {

  operators = signal<OperatorViewModel[]>([]);
  deleteDialogTitle = signal('');
  deleteDialogMessage = signal('');
  inMenuMode = signal(false);
  skills = signal<OperatorSkill[]>([]);
  skillTypes = signal<SkillTypeModel[]>([]);
  
  skillTypeToModel = skillTypeToModel;
  skillToOperatorSkill = skillToOperatorSkill;
  TranslationConstants = TranslationConstants;
  
  constructor(
    private appStoreService: AppStoreService,
    private dialog: MatDialog,
    private router: Router,
    private translate: TranslateService){
    }
    
    ngOnInit(): void {
      this.appStoreService.operators$
      .subscribe(
        (operators) => (this.operators.update(_=> operators))
      );
      
      this.appStoreService.skills$.subscribe(
        allSkills => this.skills.update(_=> allSkills)
      );

      this.appStoreService.skillTypes$.subscribe(types => this.skillTypes.update(_=> types.map(skillTypeToModel)));

    this.translate
    .get([
      TranslationConstants.OPERATORS_MANAGEMENT.DELETE_TITLE,
      TranslationConstants.OPERATORS_MANAGEMENT.DELETE_MESSAGE,
    ]).subscribe(translations => {
      this.deleteDialogMessage = translations[TranslationConstants.OPERATORS_MANAGEMENT.DELETE_MESSAGE];
      this.deleteDialogTitle = translations[TranslationConstants.OPERATORS_MANAGEMENT.DELETE_TITLE];
    });

  }

  updateMenuMode(value: boolean) {
     this.inMenuMode.update(_=> value);
    }

  onDeleteClick(operator: OperatorViewModel) {

    const dialogRef = this.dialog.open(ConfirmationDialogComponent,{
      data:{
        dialogMessage: this.deleteDialogMessage,
        dialogTitle: this.deleteDialogTitle,
        dialogResult: 'NO'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if(result.dialogResult === 'NO') return;

      this.appStoreService.deleteOperator(operator);
    });
  }

  onAddClick(){
    const dialogResult = this.dialog.open(AddOperatorComponentDialog);
    //navigate to operator details
    dialogResult.afterClosed()
    .subscribe((result: OperatorViewModel) => 
      setTimeout(() => this.router.navigate(['/management/operator/details/',`${result.data.identifier}`]),500)
    );
  }

  getSkillsForOperator(id: string){
    return this.skills().filter(x => x.operatorId === id);
  }
}
