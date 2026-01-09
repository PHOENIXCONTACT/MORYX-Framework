import { Component, signal } from '@angular/core';
import { TranslationConstants } from '../extensions/translation-constants.extensions';
import { SkillType } from '../models/skill-type-model';
import { getDurationInDays } from '../models/utils';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from '../dialogs/confirmation-dialog/confirmation-dialog.component';
import { OperatorSkill } from '../models/operator-skill-model';
import { AppStoreService } from '../services/app-store.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector: 'app-skill-types',
    templateUrl: './skill-types.component.html',
    styleUrl: './skill-types.component.scss',
    standalone: true,
    imports: [
      CommonModule,
      MatTooltipModule,
      MatIconModule,
      RouterLink,
      TranslateModule,
      MatTableModule,
      MatButtonModule
    ]
})
export class SkillTypesComponent {
  skillTypes = signal<SkillType[]>([]);
  skills = signal<OperatorSkill[]>([]);
  
  getDurationInDays = getDurationInDays;
  dataSource!: MatTableDataSource<SkillType>;
  TranslationConstants = TranslationConstants;
  displayedColumns: string[] = ['name','duration','trainedOperators','actions'];
  
  constructor(private dialog: MatDialog,
    private appStoreService: AppStoreService,
    public translate: TranslateService,
  ){

    this.appStoreService
    .skillTypes$.subscribe(results => {
      this.skillTypes.update(_=> results);
      this.dataSource = new MatTableDataSource(results)
    });

    this.appStoreService
    .skills$
    .subscribe(skills => this.skills.update(_=> skills));
  }

  async onDeleteClick(skillType: SkillType) {
    const translations = await this.translate
          .get([
            TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_TYPE_TITLE,
            TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_TYPE_MESSAGE
          ])
          .toAsync();

    const dialogRef = this.dialog.open(ConfirmationDialogComponent,{
      data:{
        dialogMessage: translations[TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_TYPE_MESSAGE],
        dialogTitle: translations[TranslationConstants.CONFIRMATION_DIALOG.DELETE_SKILL_TYPE_TITLE],
        dialogResult: 'NO'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if(result.dialogResult === 'NO') return;

      this.appStoreService.deleteSkillType(skillType);
    });
  }

  operatorWithSkillCount(typeId: number){
    return this.skills().filter(x => x.typeId === typeId).length;
  }

}
