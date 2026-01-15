/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { AfterContentChecked, Component, input } from "@angular/core";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { FormControlService } from "src/app/services/form-control-service.service";
import { ResourceModel } from "../../../api/models";
import { MatDividerModule } from "@angular/material/divider";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";

@Component({
  selector: "app-details-header",
  templateUrl: "./details-header.component.html",
  styleUrls: ["./details-header.component.scss"],
  imports: [
    CommonModule,
    FormsModule,
    MatDividerModule,
    MatFormFieldModule,
    MatInputModule,
    TranslateModule,
  ],
  standalone: true,
})
export class DetailsHeaderComponent implements AfterContentChecked {
  resource = input.required<ResourceModel>();
  editMode = input<boolean>(false);
  TranslationConstants = TranslationConstants;

  constructor(
    public translate: TranslateService,
    private formControlService: FormControlService
  ) { }

  ngAfterContentChecked(): void {
    this.formControlService.onCanSave(
      this.resource().name?.length ? true : false
    );
  }
  onNameChanged() {
    this.formControlService.onCanSave(
      this.resource().name?.length ? true : false
    );
  }
}

