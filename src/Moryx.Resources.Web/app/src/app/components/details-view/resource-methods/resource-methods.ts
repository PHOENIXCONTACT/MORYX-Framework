/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, inject, signal } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { EditResourceService } from '../../../services/edit-resource.service';
import { Router } from '@angular/router';
import { ResourceModificationService } from '../../../api/services/resource-modification.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SnackbarService } from '@moryx/ngx-web-framework/services';

import {
  Entry,
  EntryValue,
  EntryValueType,
  EntryEditor,
  NavigableEntryEditor,
  PrototypeToEntryConverter,
  MethodEntry
} from '@moryx/ngx-web-framework/entry-editor';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-resource-methods',
  templateUrl: './resource-methods.html',
  styleUrls: ['./resource-methods.scss'],
  imports: [
    EntryEditor,
    MatExpansionModule,
    NavigableEntryEditor,
    TranslateModule,
    MatButtonModule
  ]
})
export class ResourceMethods {
  private router = inject(Router);
  private resourceModificationService = inject(ResourceModificationService);
  private snackBar = inject(MatSnackBar);
  private snackbarService = inject(SnackbarService);

  private editResourceService = inject(EditResourceService);

  public methods = signal<MethodEntry[] | undefined | null>([]);
  private resourceId?: number;

  selectedMethod = signal<MethodEntry | undefined>(undefined);
  methodResult = signal<Entry | undefined>(undefined);
  resultView = signal(false);

  TranslationConstants = TranslationConstants;

  constructor() {
    effect(() => {
      const resource = this.editResourceService.activeResource();
      if (!resource) {
        return;
      }

      this.selectedMethod.set(undefined);
      this.methods.set(resource.methods);
      this.resourceId = resource.id;
      this.snackBar.dismiss();
    });
  }

  onMethodSelected(method: MethodEntry) {
    this.selectedMethod.set(method);
    const newUrl = this.getUrl();
    this.resultView.set(false);
    this.snackBar.dismiss();
    this.router.navigate([newUrl]);
  }

  private getUrl(): string {
    const url = this.router.url;
    const index = url.lastIndexOf('methods');
    let newUrl = url.substring(0, index);
    newUrl += 'methods';
    return newUrl;
  }

  public onInvoke(method: MethodEntry) {
    if (!method.name) {
      return;
    }

    let param = {};
    if (method.parameters?.subEntries && method.parameters.subEntries.length > 0) {
      for (let p of method.parameters.subEntries) {
        if (!p.value) {
          return;
        }

        if (!p.value.current) {
          if (p.value.default) {
            p.value.current = p.value.default;
          } else if (p.value.type === EntryValueType.Boolean) {
            p.value.current = 'false';
          } else {
            this.snackBar.open(
              'The parameter ' + p.displayName + ' has no value',
              'DISMISS'
            );
            return;
          }
        }
      }

      PrototypeToEntryConverter.convertToEntry(method.parameters);

      param = {
        id: this.resourceId,
        method: method.name,
        body: method.parameters,
      };

    } else {
      param = {
        id: this.resourceId,
        method: method.name,
        body: {},
      };
    }

    this.resourceModificationService
      .invokeMethod(param as { id: number; method: string; body?: Entry | undefined })
      .subscribe({
        next: (result) => {
          const resultEntry = result
            ? ({
              subEntries: [result] as Entry[],
              identifier: 'root',
              value: {type: EntryValueType.Class} as EntryValue,
            } as Entry)
            : undefined;
          this.methodResult.set(resultEntry)
          this.resultView.set(true);
        },
        error: async (e: HttpErrorResponse) =>
          await this.snackbarService.handleError(e),
      });
  }

  onChangeToParameters(method: MethodEntry) {
    this.resultView.set(false);
    this.snackBar.dismiss();
    //clear the entry parameter values for boolean types
    if (method.parameters?.subEntries?.length) {
      for (let p of method.parameters.subEntries) {
        if (p.value?.type === EntryValueType.Boolean)
          p.value.current = 'false';
      }
    }
  }

}
