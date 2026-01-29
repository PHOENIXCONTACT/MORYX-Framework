/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuTrigger, MatMenu, MatMenuContent, MatMenuItem } from '@angular/material/menu';
import { Router } from '@angular/router';
import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { environment } from 'src/environments/environment';
import { ContentDescriptorModel } from '../../api/models';
import { DialogDeleteComponent } from '../../dialogs/dialog-delete/dialog-delete.component';
import { MediaService } from '../../services/media-service/media.service';
import {
  EmptyStateComponent,
  MoryxSnackbarService,
  SearchBarService,
  SearchRequest,
  SearchSuggestion,
} from '@moryx/ngx-web-framework';
import { NgStyle, CommonModule } from '@angular/common';
import { FileDragAndDropDirective } from '../../extensions/file-drag-and-drop.directive';
import { MediaFileComponent } from './media-file/media-file.component';
import { MatIcon } from '@angular/material/icon';
import { MatFabButton } from '@angular/material/button';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
@Component({
    selector: 'app-media-overview',
    templateUrl: './media-overview.component.html',
    styleUrls: ['./media-overview.component.scss'],
    imports: [
      FileDragAndDropDirective, NgStyle, CommonModule, 
      MediaFileComponent,MatMenu, MatMenuContent, 
      MatMenuItem, MatIcon, MatMenuTrigger, 
      MatFabButton, MatProgressSpinner, TranslateModule,
      EmptyStateComponent]
})
export class MediaOverviewComponent implements OnInit, OnDestroy {
  filteredContents = signal<ContentDescriptorModel[]>([]);
  backgroundImgPath = signal(environment.assets + 'assets/moryx_transparent_colored.png');
  isLoading = signal(true);
  contents = signal<ContentDescriptorModel[]>([]);
  selectedContent = signal<string | undefined>(undefined);

  @ViewChild(MatMenuTrigger) trigger!: MatMenuTrigger;
  TranslationConstants = TranslationConstants;
  menuTopLeftPosition = { x: '0', y: '0' };

  constructor(
    public dialog: MatDialog,
    private mediaService: MediaService,
    private router: Router,
    public translate: TranslateService,
    private searchBarService: SearchBarService,
    private moryxSnackbar: MoryxSnackbarService
  ) { }

  ngOnDestroy(): void {
    this.searchBarService.unsubscribe();
  }

  ngOnInit() {
    this.mediaService.contents.subscribe({
      next: (data) => {
        this.contents.update(_ => data);
        this.filteredContents.update(_ => data);
        this.isLoading.update(_ => false);
      },
      error: async (err) => {
        const translations = await this.translate
          .get([TranslationConstants.MEDIA_OVERVIEW.FAILED_LOADING])
          .toAsync();
        this.moryxSnackbar.showError(
          translations[TranslationConstants.MEDIA_OVERVIEW.FAILED_LOADING]
        );
        this.isLoading.update(_ => false);
      },
    });

    this.mediaService.loadContents();

    this.searchBarService.subscribe({
      next: (request: SearchRequest) => {
        this.onSearch(request);
      },
    });
  }

  onSearch(result: SearchRequest) {
    const urlBase = 'Media/details/';
    var contents = this.contents().filter((c) =>
      c.name?.toLowerCase()?.includes(result.term.toLowerCase())
    );
    if (!contents) return;

    if (result.submitted) {
      this.searchBarService.clearSuggestions();
      this.filteredContents.update(_ => contents);
      this.searchBarService.subscribe({
        next: (newRequest: SearchRequest) => {
          this.onSearch(newRequest);
        },
      });
    } else {
      const searchSuggestions = [] as SearchSuggestion[];
      for (let content of contents) {
        if (!content.name) continue;

        const url = urlBase + content.id;
        searchSuggestions.push({ text: content.name, url: url });
      }
      this.filteredContents.update(_ => contents);
      this.searchBarService.provideSuggestions(searchSuggestions);
    }
  }

  onOpenMenuOnTouch(event: any, content:ContentDescriptorModel){
    this.trigger.menuData = { content: content };
    this.menuTopLeftPosition.x = event.pointers[0].clientX + 'px';
    this.menuTopLeftPosition.y = event.pointers[0].clientY + 'px';
    this.trigger.openMenu();
  }

  onSelectMedia(content: ContentDescriptorModel) {
    this.selectedContent.update( _ =>  content.id);
    this.router.navigate(['/details/', content.id]);
  }


  //upload new content
  onUpload(event: any) {
    for (var fileElement of event.target.files) {
      const file: File = fileElement;
      if (file) {
        this.mediaService.uploadContent(file);
      }
    }
  }

  //open dialog in order to check if content should really be deleted
  async onDelete(content: ContentDescriptorModel): Promise<void> {
    if (content !== undefined) {
      const deleteMessage = await this.translate
        .get(TranslationConstants.MEDIA_OVERVIEW.DELETE_MESSAGE)
        .toAsync();
      const dialogRef = this.dialog.open(DialogDeleteComponent, {
        data: {
          type: 'Content',
          deleteMessage: deleteMessage,
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result === true) this.remove(content);
      });
    }
  }

  //remove content
  remove(content: ContentDescriptorModel) {
    if (typeof content.id === 'string') {
      this.mediaService.removeContent(content.id).subscribe(() => {
        var index = this.contents().findIndex((c) => c.id === content.id);
        if (index > -1) {
          this.contents.update(items => {
            items.splice(index, 1);
            return items;
          });
          this.selectedContent.update(_ => undefined);
        }
      });
    }
  }

  fileDropped(arg: any) {
    this.onUpload(arg);
  }
}

