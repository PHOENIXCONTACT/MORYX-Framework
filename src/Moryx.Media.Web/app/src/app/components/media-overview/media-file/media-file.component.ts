/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnInit, model, signal, Output, EventEmitter } from '@angular/core';
import { ContentDescriptorModel } from '../../../api/models';
import { MediaService } from '../../../services/media-service/media.service';
import { environment } from 'src/environments/environment';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { retry } from 'rxjs';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { MatCardModule } from '@angular/material/card';
import { CommonModule } from '@angular/common';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { TranslateModule } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'app-media-file',
    templateUrl: './media-file.component.html',
    styleUrls: ['./media-file.component.scss'],
    imports: [
      CommonModule,
      MatProgressSpinner,
      TranslateModule,
      MatCardModule,
      MatButtonModule,
      MatIconModule]
})
export class MediaFileComponent implements OnInit {

  TranslationConstants = TranslationConstants;
  name = model.required<string>()
  amount = model.required<string>();
  content = model.required<ContentDescriptorModel>();
  selected = model.required<boolean>();
  loaded = signal(false);
  path = signal<string | null | ArrayBuffer>('');

  @Output() show = new EventEmitter<ContentDescriptorModel>()
  @Output() delete = new EventEmitter<ContentDescriptorModel>()
  img: any;

  constructor(
    private mediaService: MediaService,private snackbarService: SnackbarService) { }

  ngOnInit(): void {
    this.showFile();
  }

  //Shows preview if media is an image. If not, the default picture will be shown
  showFile() {
    const content = this.content();
    if (
      content.master !== undefined &&
      typeof content.id === 'string'
    ) {
      if (
        typeof content.master.mimeType === 'string' &&
        content.master.mimeType.includes('image') &&
        typeof content.master.name === 'string'
      ) {
        this.mediaService
          .getPicture(content.master.name, content.id, true)
          .pipe(
            retry({
              count: 3,
              delay:1000,
              resetOnSuccess: true
            })
          )
          .subscribe({
            next : (data) => {
              if (data !== null) {
                let downloadedFile = new Blob([data], { type: data.type });
                const reader = new FileReader();
                reader.readAsDataURL(downloadedFile); //FileStream response from .NET core backend
                reader.onload = (_event) => {
                  this.path.update(_ => reader.result) //url declared earlier
                };
                this.loaded.update(_ => true);
              }
            },
            error : error => this.snackbarService.handleError(error)
          });
      } else {
        this.path.update(_ => environment.assets + 'assets/no_preview.jpg');
        this.loaded.update(_ => true);
      }
    }
  }

  onClick(event: MouseEvent) {
    if((<HTMLElement>event.target).nodeName === 'MAT-ICON')
      this.delete.emit(this.content());
    else
      this.show.emit(this.content());
  }
}

