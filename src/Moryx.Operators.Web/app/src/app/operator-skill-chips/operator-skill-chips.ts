/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, input, ChangeDetectionStrategy } from "@angular/core";
import { OperatorSkill } from "../models/operator-skill-model";
import { SkillTypeModel } from "@api/models/skill-type-model";

import { MatTooltipModule } from "@angular/material/tooltip";
import {MatChipsModule} from '@angular/material/chips';
@Component({
    selector: "app-operator-skill-chips",
    templateUrl: "./operator-skill-chips.html",
    styleUrl: "./operator-skill-chips.scss",
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [
    MatTooltipModule,
    MatChipsModule
]
})
export class OperatorSkillChips {
  readonly operatorId = input.required<string>();
  readonly skills = input.required<OperatorSkill[]>();
  readonly skillTypes = input.required<SkillTypeModel[]>();
  readonly useTagStyle = input<boolean>();
  protected operatorSkills = computed(()=> this.skills().filter(x => x.operatorId === this.operatorId()));

  protected findSkillTypeById(id: number){
    return this.skillTypes().find(x => x.id === id);
  }

  protected skillTooltipText(){
    let skillNameArray: string[] = [];
    skillNameArray = this.operatorSkills().map((x) => this.findSkillTypeById(x.typeId)?.name ?? 'UNKNOWN');
    return skillNameArray.join(', ');
  }
}

