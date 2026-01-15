/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable, NgZone } from '@angular/core';
import { InstructionItemModel, InstructionModel } from '../api/models';
import { VisualInstructionsService } from '../api/services';
import { BehaviorSubject } from 'rxjs';
import { DisplayedMediaContent } from '../components/media-contents/displayed-media-content';
import { HttpErrorResponse, HttpClient, HttpRequest, HttpEvent, HttpEventType } from '@angular/common/http';
import { DomSanitizer } from '@angular/platform-browser';
import { environment } from 'src/environments/environment';
import { MoryxSnackbarService } from '@moryx/ngx-web-framework';

@Injectable({
  providedIn: 'root',
})
export class InstructionService {

  private eventSource?: EventSource;

  private _instructions = new BehaviorSubject<InstructionModel[]>([]);
  public instructions$ = this._instructions.asObservable();

  constructor(private zone: NgZone, 
    private visualInstructionsService: VisualInstructionsService,
    private httpClient: HttpClient,
    private snackbar: MoryxSnackbarService, 
    private sanitizer: DomSanitizer) {
    this.subscribeToStream();
  }

  public subscribeToStream() {
    if(this.eventSource) {
      this.eventSource.close();
      this._instructions.next([]);
    }

    this.eventSource = new EventSource(this.visualInstructionsService.rootUrl + '/api/moryx/instructions/stream', { withCredentials: !environment.production });
    this.eventSource.onmessage = event => {
      const instructions = JSON.parse(event.data);
      this.zone.run(() => this._instructions.next(instructions));
    };
  }

  async requestMediaContentsAsync(mediaItems: InstructionItemModel[]) : Promise<DisplayedMediaContent[]> {
    return await Promise.all(mediaItems.map(async (i) => await this.requestMediaContentAsync(i)));
  }

  async requestMediaContentAsync(mediaItem: InstructionItemModel) : Promise<DisplayedMediaContent> {
    return await this.httpClient.request<Blob>(
      new HttpRequest('GET', mediaItem.content ?? environment.assets + 'assets/moryx_transparent_colored.png', null, {
            reportProgress: true,
            responseType: 'blob',
        })
    ).toAsync()
    .then((response) => {
      return this.convertBlobResponse(response);
    })
    .catch((error) => {
      return this.handleInstructionError(error);
    });
  }

  private async handleInstructionError(e: HttpErrorResponse) : Promise<DisplayedMediaContent> {
    await this.snackbar.handleError(e);
    return { type: 'undefined', url: environment.assets + 'assets/broken_image.png' } as DisplayedMediaContent;
  }

  private convertBlobResponse(data: HttpEvent<Blob>): DisplayedMediaContent {
    if (data.type != HttpEventType.Response || data.body == null) 
      return { type: 'undefined', url: environment.assets + 'assets/broken_image.png' } as DisplayedMediaContent;

    const downloadedFile = new Blob([data.body], { type: data.body.type });
    const url = window.URL.createObjectURL(downloadedFile);
    return {
      type: data.body?.type,
      url: data.body?.type == 'application/pdf' || 'text/html' ? url : this.sanitizer.bypassSecurityTrustUrl(url),
    } as DisplayedMediaContent;
  }
}
