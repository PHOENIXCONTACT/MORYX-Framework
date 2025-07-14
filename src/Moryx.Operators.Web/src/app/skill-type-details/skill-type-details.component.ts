import { Component, effect, input, OnInit, signal, untracked } from "@angular/core";
import { TranslationConstants } from "../extensions/translation-constants.extensions";
import { SkillType } from "../models/skill-type-model";
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Entry, EntryValueType, NavigableEntryEditorComponent } from "@moryx/ngx-web-framework";
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { firstValueFrom } from "rxjs";
import { AppStoreService } from "../services/app-store.service";
import { CommonModule } from "@angular/common";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatIconModule } from "@angular/material/icon";
import { MatFormFieldModule } from "@angular/material/form-field";
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MatButtonModule } from "@angular/material/button";
import { MatInputModule } from "@angular/material/input";

@Component({
    selector: "app-skill-type-details",
    templateUrl: "./skill-type-details.component.html",
    styleUrl: "./skill-type-details.component.scss",
    standalone: true,
    imports: [
      CommonModule,
      ReactiveFormsModule,
      FormsModule,
      MatSidenavModule,
      MatTooltipModule,
      MatIconModule,
      MatFormFieldModule,
      NavigableEntryEditorComponent,
      TranslateModule,
      MatButtonModule,
      MatInputModule,
      RouterLink
    ]
})
export class SkillTypeDetailsComponent implements OnInit {
  id = input.required<number>();
  skillType = signal<SkillType>(<SkillType>{
    id: 0,
    name: "",
    acquiredCapabilities: <Entry>{
      value: {
        type: EntryValueType.Class,
      },
    },
  });

  form = new FormGroup({
    name: new FormControl("", [Validators.required]),
    duration: new FormControl(0, [Validators.min(1)]),
  });
  
  TranslationConstants = TranslationConstants;
  
  constructor(
    private activatedRoute: ActivatedRoute,
    private route: Router,
    private appStoreService: AppStoreService
  ) {
    effect(() =>{
      const id = this.id();
      untracked(() => this.initialize(id));
    })
  }

  ngOnInit(): void {
  }

  initialize(id : number){
    
    if(id <= 0){
      this.appStoreService.getSkillTypePrototype().then((prototype) => {
        this.skillType.update(_=> <SkillType>{
          id: 0,
          name: "",
          acquiredCapabilities: prototype.capabilities,
          duration: "",
        });
      });
      return;
    }


    const resultsAsync = firstValueFrom(
      this.appStoreService.getSkillType(id)
    );

    resultsAsync.then((skillType) => {
      const skillData = skillType;
      this.form.patchValue({
        name: skillData.name,
        duration: Number(skillData.duration?.split(".")[0] ?? 0),
      });

      this.skillType.update(_=> <SkillType>{
        id: skillData.id,
        name: skillData.name,
        acquiredCapabilities: skillData.capabilities,
        duration: skillData.duration,
      });
    });
  }
  onSave() {
    this.skillType.update(skill =>{
      skill.name = this.form.value.name ?? "";
      skill.duration = `${Number(this.form.value.duration)}.00:00:00`;
      return skill;
    })
    if (this.skillType()?.id <= 0) {
      this.appStoreService
        .newSkillType(this.skillType())
        ?.then((addedType) =>
          this.route.navigate(["skill-types", addedType.id])
        );
    } else {
      this.appStoreService.updateType(this.skillType());
    }
  }
}
