/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, effect, ElementRef, inject, linkedSignal, resource, signal, untracked, ViewChild } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from "@angular/material/dialog";
import { TranslateModule } from "@ngx-translate/core";
import { lastValueFrom, map, Observable, startWith } from "rxjs";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { OperationViewModel } from "../../models/operation-view-model";
import { AdviceContext, AdviceModel, ProductPartModel, StagingIndicator } from '../../api/models';
import { CommonModule } from "@angular/common";
import { MatGridListModule } from "@angular/material/grid-list";
import { MatFormFieldModule } from "@angular/material/form-field";
import { FormControl, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatListModule } from "@angular/material/list";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatButtonModule } from "@angular/material/button";
import { MatInputModule } from "@angular/material/input";
import { OrderManagementService } from "src/app/api/services";
import { AdviceOperation$Params } from "src/app/api/functions";
import { SnackbarService } from "@moryx/ngx-web-framework/services";
import { HttpErrorResponse } from "@angular/common/http";
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatIcon } from "@angular/material/icon";

@Component({
  selector: "app-advice-dialog",
  templateUrl: "./advice-dialog.html",
  styleUrls: ["./advice-dialog.scss"],
  imports: [
    CommonModule,
    MatDialogModule,
    TranslateModule,
    MatGridListModule,
    MatFormFieldModule,
    FormsModule,
    MatListModule,
    MatProgressBarModule,
    MatButtonModule,
    MatInputModule,
    MatAutocompleteModule,
    ReactiveFormsModule,
    MatIcon
]
})
export class AdviceDialog {
  @ViewChild('input') input!: ElementRef<HTMLInputElement>;
  
  TranslationConstants = TranslationConstants;
  private apiService = inject(OrderManagementService);
  private dialog = inject(MatDialogRef<AdviceDialog>);
  private snackbarService = inject(SnackbarService);
  data = inject<AdviceDialogData>(MAT_DIALOG_DATA);

  private guid = computed(() => this.data.operation.model.identifier!);
  mainProduct = computed(() => (<AdvicePartModel>{
    Identifier: this.data.operation.model.productIdentifier,
    Revision: this.data.operation.model.productRevision,
    Name: this.data.operation.model.productName
  }));

  private adviceLoader = resource<AdviceContext, { guid: string }>({
    params: () => ({ guid: this.guid() }),
    loader: ({ params }) => lastValueFrom(this.apiService.getAdviceContext({ guid: params.guid }))
  });
  alreadyAdvicedAmount = computed(() => {
    if (this.adviceLoader.hasValue()) 
      return this.adviceLoader.value().advicedAmount;
    return 0;
  });
  private partsLoader = resource<ProductPartModel[], { guid: string }>({
    params: () => ({ guid: this.guid() }),
    loader: ({ params }) => {
      if (!!this.data.operation.model.hasPartList) {
        return lastValueFrom(this.apiService.getProductParts({ guid: params.guid }))
      }
      else {
        return Promise.resolve([]);
      }
    }
  });
  private possibleParts = computed(() => {
    if (!this.partsLoader.hasValue()) {
      return [];
    }

    const parts = this.partsLoader.value()
      .filter(part => part.stagingIndicator == StagingIndicator.PickPart)
      .map(part => <AdvicePartModel>{ Id: part.id, Identifier: part.identifier, Revision: 0, Name: part.name });
    
    if (!parts.length) {
      return [];
    }

    return [this.mainProduct(), ...parts];
  });
  hasPickParts = computed(() => this.possibleParts().length);

  isLoading = signal(false);
  toteBoxNumber = signal<string | undefined>(undefined);
  amount = signal<number>(0);
  part = signal<AdvicePartModel>(this.mainProduct());
  canAdvice = computed(() => this.toteBoxNumber() && this.amount() && this.part() && !this.isLoading());

  partFormControl = new FormControl<AdvicePartModel | string>(this.mainProduct());
  filteredParts = linkedSignal(this.possibleParts)

  constructor() {
    effect(() => this.processAdviceContextLoading());
  }

  private processAdviceContextLoading(): void {
    const isLoading = this.adviceLoader.isLoading() || this.partsLoader.isLoading();
    const error = this.adviceLoader.error();
    untracked(() => {
      this.isLoading.set(isLoading);
      if (error) {
        this.dialog.close();
      }
    });
  }
  
  filter(): void {
    const filterValue = this.input.nativeElement.value.toLowerCase();
    const filtered = this.possibleParts().filter(p => this.toString(p).toLowerCase().includes(filterValue));
    this.filteredParts.set(filtered);
    if (filtered.length === 1) {
      this.part.set(filtered[0]);
    }
  }

  async submit() {
    this.isLoading.set(true);

    const advice = <AdviceModel>{
      toteBoxNumber: this.toteBoxNumber(),
      partId: this.part().Id,
      amount: this.amount(),
    };
    const params = <AdviceOperation$Params>{
      guid: this.guid(),
      body: advice
    };
    
    try {
      await lastValueFrom(this.apiService.adviceOperation(params));
      this.dialog.close();
    } catch (error) {
      await this.snackbarService.handleError(error as HttpErrorResponse);
    } finally {
      this.isLoading.set(false);
    }
  }

  toString(value: AdvicePartModel) {
    return `${value.Identifier}-${String(value.Revision).padStart(2, '0')} ${value.Name}`;
  }
}

export interface AdviceDialogData {
  operation: OperationViewModel;
}

interface AdvicePartModel {
  Id: number;
  Identifier: string;
  Revision: number;
  Name: string;
}
