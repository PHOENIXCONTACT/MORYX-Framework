/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, inject, OnDestroy, OnInit, signal, viewChild, ChangeDetectionStrategy } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuTrigger, MatMenu, MatMenuContent, MatMenuItem } from '@angular/material/menu';
import { Router } from '@angular/router';
import { TranslateService, TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/translation-constants';
import { environment } from '../../../environments/environment';
import { ContentDescriptorModel } from '@api/models';
import { DialogDelete } from '@app/dialogs/dialog-delete/dialog-delete';
import { MediaService } from '@app/services/media-service/media.service';
import { SnackbarService, SearchBarService, SearchRequest, SearchSuggestion, } from '@moryx/ngx-web-framework/services';
import { EmptyState } from '@moryx/ngx-web-framework/empty-state';
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
    FileDragAndDropDirective,
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

  readonly trigger = viewChild.required(MatMenuTrigger);
  protected TranslationConstants = TranslationConstants;
  protected menuTopLeftPosition = {x: '0', y: '0'};

  constructor() {
    effect(() => {
      const data = this.mediaService.contents();
      this.contents.set(data);
      this.filteredContents.set(data);
      this.isLoading.set(false);
    });
  }

  ngOnDestroy(): void {
    this.searchBarService.unsubscribe();
  }

  ngOnInit() {
    this.mediaService.loadContents();

    this.searchBarService.subscribe({
      next: (request: SearchRequest) => {
        this.onSearch(request);
      },
    });
  }

  private onSearch(result: SearchRequest) {
    const urlBase = 'Media/details/';
    const contents = this.contents().filter((c) =>
      c.name?.toLowerCase()?.includes(result.term.toLowerCase())
    );
    if (!contents) {
      return;
    }

    if (result.submitted) {
      this.searchBarService.clearSuggestions();
      this.filteredContents.set(contents);
      this.searchBarService.subscribe({
        next: (newRequest: SearchRequest) => {
          this.onSearch(newRequest);
        },
      });
    } else {
      const searchSuggestions = [] as SearchSuggestion[];
      for (const content of contents) {
        if (!content.name) {
          continue;
        }

        const url = urlBase + content.id;
        searchSuggestions.push({text: content.name, url: url});
      }
      this.filteredContents.set(contents);
      this.searchBarService.provideSuggestions(searchSuggestions);
    }
  }

  protected onOpenMenuOnTouch(event: { pointers: PointerEvent[] }, content: ContentDescriptorModel) {
    this.trigger().menuData = {content: content};
    this.menuTopLeftPosition.x = event.pointers[0].clientX + 'px';
    this.menuTopLeftPosition.y = event.pointers[0].clientY + 'px';
    this.trigger().openMenu();
  }

  protected onSelectMedia(content: ContentDescriptorModel) {
    this.selectedContent.set(content.id);
    this.router.navigate(['/details/', content.id]);
  }


  //upload new content
  protected onUpload(event: Event | { target: { files: FileList } }) {
    const files = (event.target as HTMLInputElement).files;
    if (!files) {
      return;
    }
    for (const fileElement of files) {
      const file: File = fileElement;
      if (file) {
        this.mediaService.uploadContent(file);
      }
    }
  }

  //open dialog in order to check if content should really be deleted
  protected async onDelete(content: ContentDescriptorModel): Promise<void> {
    if (content !== undefined) {
      const deleteMessage = await firstValueFrom(this.translateService
        .get(TranslationConstants.MEDIA_OVERVIEW.DELETE_MESSAGE));
      const dialogRef = this.dialog.open(DialogDelete, {
        data: {
          type: 'Content',
          deleteMessage: deleteMessage,
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result === true) {
          this.remove(content);
        }
      });
    }
  }

  //remove content
  private async remove(content: ContentDescriptorModel): Promise<void> {
    if (typeof content.id === 'string') {
      await this.mediaService.removeContent(content.id);
      this.contents.update(items => items.filter(c => c.id !== content.id));
      this.filteredContents.update(items => items.filter(c => c.id !== content.id));
      this.selectedContent.set(undefined);
    }
  }

  protected fileDropped(arg: { target: { files: FileList } }) {
    this.onUpload(arg);
  }
}
