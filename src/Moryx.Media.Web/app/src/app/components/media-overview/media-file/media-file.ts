/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnInit, model, signal, output, ChangeDetectionStrategy } from '@angular/core';
import { ContentDescriptorModel } from '@api/models';
import { MediaService } from '@app/services/media-service/media.service';
import { environment } from '../../../../environments/environment';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { retry } from 'rxjs';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { TranslatePipe } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-media-file',
  templateUrl: './media-file.html',
  styleUrls: ['./media-file.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatProgressSpinner,
    TranslatePipe,
    MatCardModule,
    MatButtonModule,
    MatIconModule]
})
export class MediaFile implements OnInit {
  private mediaService = inject(MediaService);
  private snackbarService = inject(SnackbarService);

  protected TranslationConstants = TranslationConstants;
  readonly name = model.required<string>()
  readonly amount = model.required<string>();
  readonly content = model.required<ContentDescriptorModel>();
  readonly selected = model.required<boolean>();
  protected loaded = signal(false);
  protected path = signal<string | null | ArrayBuffer>('');

  readonly show = output<ContentDescriptorModel>();
  readonly delete = output<ContentDescriptorModel>();
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
              delay: 1000,
              resetOnSuccess: true
            })
          )
          .subscribe({
            next: (data) => {
              if (data !== null) {
                const downloadedFile = new Blob([data], {type: data.type});
                const reader = new FileReader();
                reader.readAsDataURL(downloadedFile); //FileStream response from .NET core backend
                reader.onload = (event) => {
                  this.path.set(reader.result) //url declared earlier
                };
                this.loaded.set(true);
              }
            },
            error: error => this.snackbarService.handleError(error)
          });
      } else {
        this.path.set(environment.assets + 'assets/no_preview.jpg');
        this.loaded.set(true);
      }
    }
  }

  protected onClick(event: MouseEvent) {
    if ((<HTMLElement>event.target).nodeName === 'MAT-ICON') {
      this.delete.emit(this.content());
    } else {
      this.show.emit(this.content());
    }
  }
}

