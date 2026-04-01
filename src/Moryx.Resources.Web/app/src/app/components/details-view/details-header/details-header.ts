/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { AfterContentChecked, Component, inject, input } from "@angular/core";
import { TranslateModule } from "@ngx-translate/core";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { FormControlService } from "src/app/services/form-control-service.service";
import { ResourceModel } from "../../../api/models";
import { MatDividerModule } from "@angular/material/divider";

import { FormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";

@Component({
  selector: "app-details-header",
  templateUrl: "./details-header.html",
  styleUrls: ["./details-header.scss"],
  imports: [
    FormsModule,
    MatDividerModule,
    MatFormFieldModule,
    MatInputModule,
    TranslateModule
  ]
})
export class DetailsHeader implements AfterContentChecked {
  private formControlService = inject(FormControlService);

  // ToDo: Replace input with service reference
  resource = input.required<ResourceModel>();
  editMode = input<boolean>(false);
  TranslationConstants = TranslationConstants;

  constructor() {

  }

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
