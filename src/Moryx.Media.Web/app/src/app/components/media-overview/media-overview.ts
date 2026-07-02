/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, OnDestroy, OnInit, signal, viewChild, ChangeDetectionStrategy } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuTrigger, MatMenu, MatMenuContent, MatMenuItem } from '@angular/material/menu';
import { Router } from '@angular/router';
import { TranslateService, TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { environment } from '../../../environments/environment';
import { ContentDescriptorModel } from '@api/models';
import { DialogDelete } from '@app/dialogs/dialog-delete/dialog-delete';
import { MediaService } from '@app/services/media-service/media.service';
import { SnackbarService, SearchBarService, SearchRequest, SearchSuggestion, } from '@moryx/ngx-web-framework/services';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
import { NgStyle, CommonModule } from '@angular/common';
import { FileDragAndDropDirective } from '@app/extensions/file-drag-and-drop.directive';
import { MediaFile } from './media-file/media-file';
import { MatIcon } from '@angular/material/icon';
import { MatFabButton } from '@angular/material/button';
import { MatProgressSpinner } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-media-overview',
  templateUrl: './media-overview.html',
  styleUrls: ['./media-overview.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    FileDragAndDropDirective, NgStyle, CommonModule,
    MediaFile, MatMenu, MatMenuContent,
    MatMenuItem, MatIcon, MatMenuTrigger,
    MatFabButton, MatProgressSpinner, TranslatePipe,
    EmptyState]
})
export class MediaOverview implements OnInit, OnDestroy {
  private dialog = inject(MatDialog);
  private mediaService = inject(MediaService);
  private router = inject(Router);
  private translateService = inject(TranslateService);
  private searchBarService = inject(SearchBarService);
  private snackbarService = inject(SnackbarService);

  protected filteredContents = signal<ContentDescriptorModel[]>([]);
  protected backgroundImgPath = signal(environment.assets + 'assets/moryx_transparent_colored.png');
  protected isLoading = signal(true);
  protected contents = signal<ContentDescriptorModel[]>([]);
  protected selectedContent = signal<string | undefined>(undefined);

  trigger = viewChild.required(MatMenuTrigger);
  protected TranslationConstants = TranslationConstants;
  protected menuTopLeftPosition = {x: '0', y: '0'};

  constructor() {
  }

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
        const translations = await lastValueFrom(this.translateService
          .get([TranslationConstants.MEDIA_OVERVIEW.FAILED_LOADING]));
        this.snackbarService.showError(
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
    const contents = this.contents().filter((c) =>
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
        searchSuggestions.push({text: content.name, url: url});
      }
      this.filteredContents.update(_ => contents);
      this.searchBarService.provideSuggestions(searchSuggestions);
    }
  }

  onOpenMenuOnTouch(event: any, content: ContentDescriptorModel) {
    this.trigger().menuData = {content: content};
    this.menuTopLeftPosition.x = event.pointers[0].clientX + 'px';
    this.menuTopLeftPosition.y = event.pointers[0].clientY + 'px';
    this.trigger().openMenu();
  }

  protected onSelectMedia(content: ContentDescriptorModel) {
    this.selectedContent.update(_ => content.id);
    this.router.navigate(['/details/', content.id]);
  }


  //upload new content
  protected onUpload(event: any) {
    for (const fileElement of event.target.files) {
      const file: File = fileElement;
      if (file) {
        this.mediaService.uploadContent(file);
      }
    }
  }

  //open dialog in order to check if content should really be deleted
  protected async onDelete(content: ContentDescriptorModel): Promise<void> {
    if (content !== undefined) {
      const deleteMessage = await lastValueFrom(this.translateService
        .get(TranslationConstants.MEDIA_OVERVIEW.DELETE_MESSAGE));
      const dialogRef = this.dialog.open(DialogDelete, {
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
        this.contents.update(items => items.filter(c => c.id !== content.id));
        this.filteredContents.update(items => items.filter(c => c.id !== content.id));
        this.selectedContent.update(_ => undefined);
      });
    }
  }

  protected fileDropped(arg: any) {
    this.onUpload(arg);
  }
}
