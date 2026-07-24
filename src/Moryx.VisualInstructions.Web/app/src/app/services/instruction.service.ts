/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { effect, inject, Injectable, signal } from '@angular/core';
import { InstructionItemModel, InstructionModel } from '../api/models';
import { VisualInstructionsService } from '../api/services';
import { DisplayedMediaContent } from '../components/media-contents/displayed-media-content';
import { HttpErrorResponse, HttpClient, HttpRequest, HttpEvent, HttpEventType } from '@angular/common/http';
import { DomSanitizer } from '@angular/platform-browser';
import { environment } from '../../environments/environment';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class InstructionService {
  private visualInstructionsService = inject(VisualInstructionsService);
  private httpClient = inject(HttpClient);
  private snackbarService = inject(SnackbarService);
  private domSanitizer = inject(DomSanitizer);

  private eventSource?: EventSource;

  private readonly _instructions = signal<InstructionModel[]>([]);
  readonly instructions = this._instructions.asReadonly();
  
  private readonly _connected = signal<boolean>(false);
  readonly connected = this._connected.asReadonly();

  public connect() {
    this.eventSource = new EventSource(this.visualInstructionsService.rootUrl + '/api/moryx/instructions/stream', {withCredentials: !environment.production});
    this.eventSource.onmessage = event => {
      const instructions = JSON.parse(event.data);
      this._instructions.set(instructions);
      this._connected.set(true);
    };
    this.eventSource.onerror = event => {
      this._connected.set(false);
    };

  }

  async requestMediaContentsAsync(mediaItems: InstructionItemModel[]): Promise<DisplayedMediaContent[]> {
    return await Promise.all(mediaItems.map(async (i) => await this.requestMediaContentAsync(i)));
  }

  async requestMediaContentAsync(mediaItem: InstructionItemModel): Promise<DisplayedMediaContent> {
    return await firstValueFrom(this.httpClient.request<Blob>(
      new HttpRequest('GET', mediaItem.content ?? environment.assets + 'assets/moryx_transparent_colored.png', null, {
        reportProgress: true,
        responseType: 'blob',
      })
    ))
      .then((response) => {
        return this.convertBlobResponse(response);
      })
      .catch((error) => {
        return this.handleInstructionError(error);
      });
  }

  private async handleInstructionError(e: HttpErrorResponse): Promise<DisplayedMediaContent> {
    await this.snackbarService.handleError(e);
    return {type: 'undefined', url: environment.assets + 'assets/broken_image.png'} as DisplayedMediaContent;
  }

  private convertBlobResponse(data: HttpEvent<Blob>): DisplayedMediaContent {
    if (data.type != HttpEventType.Response || data.body == null) {
      return {type: 'undefined', url: environment.assets + 'assets/broken_image.png'} as DisplayedMediaContent;
    }

    const downloadedFile = new Blob([data.body], {type: data.body.type});
    const url = window.URL.createObjectURL(downloadedFile);
    return {
      type: data.body?.type,
      url: data.body?.type == 'application/pdf' || 'text/html' ? url : this.domSanitizer.bypassSecurityTrustUrl(url),
    } as DisplayedMediaContent;
  }

  disconnect() {
    this._instructions.set([]);

    if (this.eventSource) {
      this.eventSource.close();
      this.eventSource = undefined;
      this._connected.set(false);
    }
  }
}
