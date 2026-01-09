import { Component, computed, input, Input } from "@angular/core";
import { OperatorSkill } from "../models/operator-skill-model";
import { SkillTypeModel } from "../api/models/skill-type-model";
import { CommonModule } from "@angular/common";
import { MatTooltipModule } from "@angular/material/tooltip";
import {MatChipsModule} from '@angular/material/chips';
@Component({
    selector: "app-operator-skill-chips",
    templateUrl: "./operator-skill-chips.component.html",
    styleUrl: "./operator-skill-chips.component.scss",
    standalone: true,
    imports: [
      CommonModule,
      MatTooltipModule,
      MatChipsModule
    ]
})
export class OperatorSkillChipsComponent {
  operatorId = input.required<string>();
  skills = input.required<OperatorSkill[]>();
  skillTypes = input.required<SkillTypeModel[]>();
  useTagStyle = input<boolean>();
  operatorSkills = computed(()=> this.skills().filter(x => x.operatorId === this.operatorId()));

  findSkillTypeById(id: number){
    return this.skillTypes().find(x => x.id === id);
  }

  skillTooltipText(){
    let skillNameArray: string[] = [];
    skillNameArray = this.operatorSkills().map((x,index) => this.findSkillTypeById(x.typeId)?.name ?? 'UNKNOWN');
    return skillNameArray.join(', ');
  }
}
