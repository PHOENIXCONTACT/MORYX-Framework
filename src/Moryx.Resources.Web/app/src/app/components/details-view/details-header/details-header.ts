/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, inject, linkedSignal, untracked, ChangeDetectionStrategy } from "@angular/core";
import { TranslatePipe } from "@ngx-translate/core";
import { TranslationConstants } from "@app/extensions/translation-constants.extensions";
import { FormControlService } from "@app/services/form-control-service.service";
import { ResourceModel } from "../../../api/models";
import { MatDividerModule } from "@angular/material/divider";

import { FormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { EditResourceService } from "@app/services/edit-resource.service";
import { toSignal } from "@angular/core/rxjs-interop";

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
  private readonly formControlService = inject(FormControlService);
  private readonly editService = inject(EditResourceService);
  protected readonly activeResource = linkedSignal(() => this.editService.activeResource() as ResourceModel);
  protected readonly editMode = toSignal(this.editService.edit$, { initialValue: false });

  protected TranslationConstants = TranslationConstants;

  constructor() {
    effect(() => {
      const resource = this.activeResource();
      if (!resource) {
        return;
      }
      untracked(() => this.formControlService.onCanSave(!!resource.name?.length));
    });
  }

  protected onNameChanged() {
    this.formControlService.onCanSave(!!this.activeResource().name?.length);
  }

  protected updateResource() {
    this.editService.updateActiveResource(this.activeResource());
  }
}
