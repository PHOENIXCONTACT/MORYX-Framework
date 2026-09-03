/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, inject, input, signal, untracked, ChangeDetectionStrategy } from '@angular/core';
import { TranslationConstants } from '@app/translation-constants';
import { DisplayedMediaContent } from './displayed-media-content';
import { NgxDocViewerModule } from 'ngx-doc-viewer';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-media-contents',
  templateUrl: './media-contents.html',
  styleUrls: ['./media-contents.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    NgxDocViewerModule,
    MatIconModule,
    MatButtonModule
  ]
})
export class MediaContents {

  protected medias = signal<DisplayedMediaContent[]>([]);
  readonly displayedContents = input.required<DisplayedMediaContent[]>();
  protected selectedContent = signal<DisplayedMediaContent | undefined>(undefined);
  private sanitizer = inject(DomSanitizer);
  protected TranslationConstants = TranslationConstants;

  constructor() {
    effect(() => {
      const contentData = this.displayedContents();
      untracked(() => {
        this.medias.set(contentData);
        this.selectedContent.set(contentData[0]);
      })
    })
  }

  protected onSelect(selected: DisplayedMediaContent): void {
    this.selectedContent.set(selected);
  }

  protected onNext() {
    const currentIndex = this.medias().findIndex(c => c.url === this.selectedContent()?.url);
    const nextIndex = (1 + currentIndex) % this.medias().length;
    this.selectedContent.set(this.medias()[nextIndex]);
  }

  protected onPrevious() {
    const currentIndex = this.medias().findIndex(c => c.url === this.selectedContent()?.url);
    const previousIndex = (this.medias().length - 1 + currentIndex) % this.medias().length;
    this.selectedContent.set(this.medias()[previousIndex]);
  }

  protected getSafeUrl(url: string): SafeResourceUrl {
    return this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }
}


