/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { BehaviorSubject, catchError, Observable, of } from 'rxjs';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ContentDescriptorModel, VariantDescriptor } from '../../api/models';
import { MediaServerService } from '../../api/services';

@Injectable({
  providedIn: 'root',
})
export class MediaService {
  private mediaServerService = inject(MediaServerService);
  private snackbarService = inject(SnackbarService);

  contents: BehaviorSubject<ContentDescriptorModel[]> = new BehaviorSubject<ContentDescriptorModel[]>([] as ContentDescriptorModel[]);
  TranslationConstants = TranslationConstants;

  loadContents(): void {
    this.mediaServerService
      .getAll()
      .pipe(
        catchError(
          this.handleError<ContentDescriptorModel[]>('Retrieving Contents', [])
        )
      )
      .subscribe((response) => {
        this.contents.next(response);
      });
  }

  loadContent(id: string): Observable<ContentDescriptorModel> {
    return this.mediaServerService
      .get({guid: id})
      .pipe(
        catchError(
          this.handleError<ContentDescriptorModel>('Retrieving Contents')
        )
      );
  }

  getContent(id: string): ContentDescriptorModel | undefined {
    let contentValues = this.contents.getValue();
    return contentValues.find((c) => c.id === id);
  }

  removeContent(id: string): Observable<any> {
    return this.mediaServerService
      .removeContent({guid: id})
      .pipe(catchError(this.handleError<any>('Removing content')));
  }

  removeVariant(id: string, variantName: string): Observable<any> {
    return this.mediaServerService
      .removeVariant({guid: id, variantName: variantName})
      .pipe(catchError(this.handleError<any>('Removing variant')));
  }

  uploadContent(file: File): void {
    this.mediaServerService.addMaster({body: {formFile: file}}).subscribe({
      next: (data) => {
        this.loadContent(data).subscribe({
          next: (x) => {
            if (x !== undefined) {
              // Delay until server generate the preview
              this.wait(1000).then(() => {
                this.contents.value.push(x);
              });
            }
          },
        });
      },
      error: async (err: HttpErrorResponse) => {
        await this.snackbarService.handleError(err);
      },
    });
  }

  uploadVariant(
    id: string,
    variantName: string,
    file: File
  ): Observable<string> {
    return this.mediaServerService
      .addVariant({
        contentId: id,
        variantName: variantName,
        body: {formFile: file},
      })
      .pipe(
        catchError(this.handleError<string>('Upload variant', {} as string))
      );
  }

  getPicture(
    variantName: string,
    contentGuid: string,
    preview: boolean
  ): Observable<Blob> {
    return this.mediaServerService
      .getVariantStream$Json({
        guid: contentGuid,
        variantName: variantName,
        preview: preview,
      })
  }

  getVariant(
    variantName: string,
    contentGuid: string
  ): Observable<VariantDescriptor> {
    return this.mediaServerService
      .getVariant({guid: contentGuid, variantName: variantName})
      .pipe(
        catchError(this.handleError<VariantDescriptor>('Retrieving variant'))
      );
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: HttpErrorResponse): Observable<T> => {
      console.error(error);
      this.snackbarService.handleError(error);
      return of(result as T);
    };
  }

  wait(milliseconds: number) {
    return new Promise((vars) => setTimeout(vars, milliseconds));
  }
}

