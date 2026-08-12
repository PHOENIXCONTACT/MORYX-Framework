/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslationConstants } from '@app/translation-constants';
import { ContentDescriptorModel, VariantDescriptor } from '@api/models';
import { MediaServerService } from '@api/services';

@Injectable({
  providedIn: 'root',
})
export class MediaService {
  private mediaServerService = inject(MediaServerService);
  private snackbarService = inject(SnackbarService);

  private readonly _contents = signal<ContentDescriptorModel[]>([]);
  readonly contents = this._contents.asReadonly();
  protected TranslationConstants = TranslationConstants;

  async loadContents(): Promise<void> {
    try {
      const response = await this.mediaServerService.getAll();
      this._contents.set(response);
    } catch (error) {
      this.handleError<ContentDescriptorModel[]>('Retrieving Contents')(error as HttpErrorResponse);
    }
  }

  async loadContent(id: string): Promise<ContentDescriptorModel | undefined> {
    try {
      return await this.mediaServerService.get({guid: id});
    } catch (error) {
      this.handleError<ContentDescriptorModel>('Retrieving Contents')(error as HttpErrorResponse);
      return undefined;
    }
  }

  getContent(id: string): ContentDescriptorModel | undefined {
    const contentValues = this.contents();
    return contentValues.find((c) => c.id === id);
  }

  async removeContent(id: string): Promise<void> {
    try {
      await this.mediaServerService.removeContent({guid: id});
    } catch (error) {
      this.handleError<void>('Removing content')(error as HttpErrorResponse);
    }
  }

  async removeVariant(id: string, variantName: string): Promise<void> {
    try {
      await this.mediaServerService.removeVariant({guid: id, variantName: variantName});
    } catch (error) {
      this.handleError<void>('Removing variant')(error as HttpErrorResponse);
    }
  }

  async uploadContent(file: File): Promise<void> {
    try {
      const data = await this.mediaServerService.addMaster({body: {formFile: file}});
      const content = await this.loadContent(data);
      if (content) {
        // Delay until server generate the preview
        await this.wait(1000);
        this._contents.update(items => [...items, content]);
      }
    } catch (err) {
      await this.snackbarService.handleError(err as HttpErrorResponse);
    }
  }

  async uploadVariant(id: string, variantName: string, file: File): Promise<string | undefined> {
    try {
      return await this.mediaServerService.addVariant({
        contentId: id,
        variantName: variantName,
        body: {formFile: file},
      });
    } catch (error) {
      this.handleError<string>('Upload variant')(error as HttpErrorResponse);
      return undefined;
    }
  }

  async getPicture(variantName: string, contentGuid: string, preview: boolean): Promise<Blob | undefined> {
    try {
      return await this.mediaServerService.getVariantStream$Json({
        guid: contentGuid,
        variantName: variantName,
        preview: preview,
      });
    } catch (error) {
      this.handleError<Blob>('Retrieving picture')(error as HttpErrorResponse);
      return undefined;
    }
  }

  async getVariant(variantName: string, contentGuid: string): Promise<VariantDescriptor | undefined> {
    try {
      return await this.mediaServerService.getVariant({guid: contentGuid, variantName: variantName});
    } catch (error) {
      this.handleError<VariantDescriptor>('Retrieving variant')(error as HttpErrorResponse);
      return undefined;
    }
  }

  private handleError<T>(operation = 'operation') {
    return (error: HttpErrorResponse): void => {
      console.error(error);
      this.snackbarService.handleError(error);
    };
  }

  wait(milliseconds: number) {
    return new Promise((vars) => setTimeout(vars, milliseconds));
  }
}

