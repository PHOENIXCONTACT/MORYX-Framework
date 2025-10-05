import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { MoryxSerializationMethodEntry as MethodEntry } from '../api/models';
import {
  Entry,
  EntryValue,
  EntryValueType,
  MoryxSnackbarService,
  PrototypeToEntryConverter,
} from '@moryx/ngx-web-framework';
import { ResourceModificationService } from '../api/services';
import { EditResourceService } from './edit-resource.service';
import { HttpErrorResponse } from '@angular/common/http';
import { TranslationConstants } from '../extensions/translation-constants.extensions';

@Injectable({
  providedIn: 'root',
})
export class ResourceMethodService {
  public methods: MethodEntry[] | undefined | null;
  public selectedMethod: MethodEntry | undefined;
  public resourceId?: number;
  public methodResult: Entry | undefined;
  public resultView: boolean = false;
  TranslationConstants = TranslationConstants;
  constructor(
    private editService: EditResourceService,
    private router: Router,
    private service: ResourceModificationService,
    private snackBar: MatSnackBar,
    private moryxSnackbar: MoryxSnackbarService,
  ) {
    this.editService.activeResource$.subscribe((resource) => {
      if (!resource) 
        return;

      this.selectedMethod = undefined;
      this.methods = resource.methods;
      this.resourceId = resource.id;
      this.snackBar.dismiss();
    });
  }

  private getUrl(): string {
    let url = this.router.url;
    const index = url.lastIndexOf('methods');
    var newUrl = url.substring(0, index);
    newUrl += 'methods';
    return newUrl;
  }

  onSelect(method: MethodEntry) {
    this.selectedMethod = method;
    var newUrl = this.getUrl();
    this.resultView = false;
    this.snackBar.dismiss();
    this.router.navigate([newUrl]);
  }

  public onInvoke(method: MethodEntry) {
    if (!method.name) return;

    var param = {};
    if (
      method.parameters?.subEntries &&
      method.parameters.subEntries.length > 0
    ) {
      for (var p of method.parameters.subEntries) {
        if(!p.value) return;
        if (!p.value.current) {
          if (p.value.default) p.value.current = p.value.default;
          else if (p.value.type === EntryValueType.Boolean)
            p.value.current = 'false';
          else {
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
    this.service
      .invokeMethod(
        param as { id: number; method: string; body?: Entry | undefined }
      )
      .subscribe({
        next: (result) => {
          this.methodResult = result
            ? ({
                subEntries: [result] as Entry[],
                identifier: 'root',
                value: { type: EntryValueType.Class } as EntryValue,
              } as Entry)
            : undefined;
          this.resultView = true;
        },
        error: async (e: HttpErrorResponse) =>
          await this.moryxSnackbar.handleError(e),
      });
  }

  onChangeToParameters(method: MethodEntry) {
    this.resultView = false;
    this.snackBar.dismiss();
    //clear the entry parameter values for boolean types
    if (method.parameters?.subEntries?.length) {
      for (var p of method.parameters.subEntries) {
          if (p.value?.type === EntryValueType.Boolean)
            p.value.current = 'false';
        }
      }
  }
}
