/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, linkedSignal, ChangeDetectionStrategy } from "@angular/core";
import { TranslatePipe } from "@ngx-translate/core";
import { TranslationConstants } from "@app/extensions/translation-constants";
import { ResourceModel } from "../../../api/models";
import { MatDividerModule } from "@angular/material/divider";

import { FormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { EditResourceService } from "@app/services/edit-resource.service";

@Component({
  selector: "app-details-header",
  templateUrl: "./details-header.html",
  styleUrls: ["./details-header.scss"],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    FormsModule,
    MatDividerModule,
    MatFormFieldModule,
    MatInputModule,
    TranslatePipe
  ]
})
export class DetailsHeader {
  private readonly editService = inject(EditResourceService);
  protected readonly activeResource = linkedSignal(() => this.editService.activeResource() as ResourceModel);
  protected readonly editMode = this.editService.editing;

  protected TranslationConstants = TranslationConstants;

  protected updateResource() {
    this.editService.updateActiveResource(this.activeResource());
  }
}
