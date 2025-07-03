import { Component, effect, input, Input, OnInit, signal, untracked } from '@angular/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { DisplayedMediaContent } from './displayed-media-content';
import { CommonModule } from '@angular/common';
import { NgxDocViewerComponent, NgxDocViewerModule } from 'ngx-doc-viewer';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector: 'media-contents',
    templateUrl: './media-contents.component.html',
    styleUrls: ['./media-contents.component.scss'],
    imports: [
      CommonModule,
      NgxDocViewerModule,
      MatIconModule,
      MatButtonModule
    ],
    standalone: true
})
export class MediaContentsComponent implements OnInit {

  medias = signal<DisplayedMediaContent[]>([]);
  displayedContents = input.required<DisplayedMediaContent[]>();
  selectedContent = signal<DisplayedMediaContent | undefined>(undefined) ;

  TranslationConstants = TranslationConstants;

  constructor(){
    effect(() => {
      const contentData = this.displayedContents();
      untracked(() => {
        this.medias.update(_ => contentData);
        this.selectedContent.update(_ => contentData[0]);
      })
    })
  }

  ngOnInit(): void {}

  onSelect(selected: DisplayedMediaContent): void {
    this.selectedContent.update(_ => selected);
  }

  onNext() {
    const currentIndex = this.medias().findIndex(c => c.url === this.selectedContent()?.url);
    const nextIndex = (1 + currentIndex) % this.medias().length;
    this.selectedContent.update(_ => this.medias()[nextIndex]);
  }

  onPrevious() {
    const currentIndex = this.medias().findIndex(c => c.url === this.selectedContent()?.url);
    const previousIndex = (this.medias().length - 1 + currentIndex) % this.medias().length;
    this.selectedContent.update(_ => this.medias()[previousIndex]);
  }
}

