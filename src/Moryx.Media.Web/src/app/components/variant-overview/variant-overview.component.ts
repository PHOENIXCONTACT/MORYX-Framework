import { Component, OnInit, signal, viewChild, ViewChild } from '@angular/core';
import { VariantDescriptor } from '../../api/models';
import { ContentDescriptorModel } from '../../api/models/content-descriptor-model';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuTrigger, MatMenu, MatMenuContent, MatMenuItem, MatMenuModule } from '@angular/material/menu';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MoryxSnackbarService } from '@moryx/ngx-web-framework';
import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { MediaServerService } from 'src/app/api/services';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { environment } from 'src/environments/environment';
import {
  AddVariantResultData,
  DialogAddVariantComponent
} from '../../dialogs/dialog-add-variant/dialog-add-variant.component';
import { DialogDeleteComponent } from '../../dialogs/dialog-delete/dialog-delete.component';
import { DialogVariantInfoComponent } from '../../dialogs/dialog-variant-info/dialog-variant-info.component';
import { MediaService } from '../../services/media-service/media.service';
import { NgIf, NgStyle, NgFor, NgClass, NgSwitch, NgSwitchCase, DecimalPipe, DatePipe, CommonModule } from '@angular/common';
import { MatSidenavContainer, MatSidenav, MatSidenavContent, MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbar, MatToolbarModule, MatToolbarRow } from '@angular/material/toolbar';
import { MatIconButton, MatFabButton, MatButtonModule } from '@angular/material/button';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { MatSelectionList, MatListOption, MatListModule } from '@angular/material/list';
import { MatTooltip } from '@angular/material/tooltip';
import { MatProgressSpinner, MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NgxDocViewerModule } from 'ngx-doc-viewer';

@Component({
    selector: 'app-variant-overview',
    templateUrl: './variant-overview.component.html',
    styleUrls: ['./variant-overview.component.scss'],
    imports: [
      MatSidenavModule, 
      MatToolbarModule, 
      MatIconModule, 
      RouterLink, 
      MatMenuModule, 
      MatListModule, 
      MatTooltip, MatButtonModule, 
      NgxDocViewerModule, 
      CommonModule, 
      TranslateModule,
      MatProgressSpinnerModule
    ]
})
export class VariantOverviewComponent implements OnInit {
  mediaImage = signal(environment.assets + 'assets/media-toolbar.webp');
  content = signal<ContentDescriptorModel | undefined>(undefined);
  selectedVariant = signal<VariantDescriptor | undefined>(undefined);
  // 0: no picture loaded, 1: picture loading, 2: picture loaded
  bigPictureLoadingState = signal(0);
  bigPictureUrl = signal<string | null | ArrayBuffer>('');
  pdfUrl = signal<string | undefined>( undefined);
  previews = signal<Map<string, string | ArrayBuffer | null>>( new Map());
  bigPictureIsPdf = signal(false);
  defaultPictureUrl = signal(environment.assets + 'assets/no_preview.jpg');
  
  downloadPictureUrl: string | null | ArrayBuffer = '';
  TranslationConstants = TranslationConstants;
  menuTopLeftPosition = signal<{ x: string, y: string }>({ x: '0', y: '0' });
  timeoutHandler: ReturnType<typeof setTimeout> | undefined;
  trigger = viewChild.required(MatMenuTrigger);


  constructor(
    public dialog: MatDialog,
    private route: ActivatedRoute,
    private mediaService: MediaService,
    private moryxSnackbar: MoryxSnackbarService,
    public translate: TranslateService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id !== null) {
      this.content.set(this.mediaService.getContent(id));

      //if contents in mediaService is still empty because of a reload, load the needed content seperately
      if (this.content() !== undefined) {
        this.loadPreviews();
      } else {
        this.mediaService.loadContent(id).subscribe((x) => {
          this.content.update(_ => x);
          if (this.content() !== undefined) this.loadPreviews();
        });
      }
    }
  }

  onContext(event: MouseEvent, variant: VariantDescriptor) {
    event.preventDefault();
    event.stopPropagation();
    this.trigger()!.menuData = { variant: variant };
    this.menuTopLeftPosition.set({x : event.clientX + 'px',y : event.clientY + 'px'});
    this.trigger()!.openMenu();
  }

  onMouseUp(event: MouseEvent, variant: VariantDescriptor) {
    if (this.timeoutHandler) {
      clearTimeout(this.timeoutHandler);
      this.timeoutHandler = undefined;
    }
  }

  onMouseDown(event: MouseEvent, variant: VariantDescriptor) {
    this.onSelect(variant);
    if (event.button != 0) return;

    this.timeoutHandler = setTimeout(() => {
      this.onContext(event, variant);
      this.timeoutHandler = undefined;
    }, 500);
  }

  loadPreviews(): void {
    const content = this.content();
    if (
      content !== undefined &&
      content.variants !== undefined &&
      content.variants !== null &&
      typeof content.id === 'string'
    ) {
      for (let variant of content.variants) {
        if (
          typeof variant.name === 'string' &&
          typeof variant.mimeType === 'string'
        )
          this.addPreview(variant.name, variant.mimeType, content.id);
      }
    }
  }

  addPreview(variantName: string, mimeType: string, contentId: string) {
    if (mimeType.includes('image')) {
      this.mediaService
        .getPicture(variantName, contentId, true)
        .subscribe((data) => {
          if (data !== null) {
            let downloadedFile = new Blob([data], { type: data.type });
            const reader = new FileReader();
            reader.readAsDataURL(downloadedFile);
            reader.onload = (_event) => {
              let url = reader.result;
              this.setMapItem(variantName, url);
            };
          }
        });
    } else {
      this.setMapItem(variantName, this.defaultPictureUrl());
    }
  }

  private setMapItem(variantName: string, url: string | ArrayBuffer | null) {
    this.previews.update(map => {
      map.set(variantName, url);
      return map;
    });
  }

  onSelect(variant: VariantDescriptor) {
    if (this.selectedVariant()?.name !== variant.name) {
      this.selectedVariant.update(_ => variant);
      this.bigPictureLoadingState.update(_ => 1);
      this.loadPicture(variant);
    }
  }

  loadPicture(variant: VariantDescriptor) {
    const content = this.content();
    if (
      content !== undefined &&
      variant !== undefined &&
      typeof variant.name === 'string' &&
      typeof content.id === 'string'
    ) {
      this.mediaService
        .getPicture(variant.name, content.id, false)
        .subscribe((data) => {
          if (data !== null) {
            const bigPicture = new Blob([data], { type: data.type });
            const reader = new FileReader();
            reader.readAsDataURL(bigPicture);
            reader.onload = (_event) => {
              this.downloadPictureUrl = reader.result;
              if (
                typeof variant.mimeType === 'string' &&
                variant.mimeType.includes('image')
              ) {
                this.bigPictureUrl.update(_ => this.downloadPictureUrl);
              } else if (
                typeof variant.mimeType === 'string' &&
                variant.mimeType.includes('application/pdf')
              ) {
                this.pdfUrl.update(_ => this.downloadPictureUrl as string);
              } else {
                this.bigPictureUrl.update(_ => this.defaultPictureUrl());
              }
              if (
                typeof variant.mimeType === 'string' &&
                variant.mimeType.includes('application/pdf')
              ) {
                this.bigPictureIsPdf.update(_ => true);
              } else {
                this.bigPictureIsPdf.update(_ => false);
              }
              this.bigPictureLoadingState.update(_ => 2);
            };
          }
        });
    }
  }

  //Adds a new variant to content and uploads the file from the PC
  onUpload(): void {
    const content = this.content();
    if (content !== undefined) {
      const dialogRef = this.dialog.open(DialogAddVariantComponent, {
        data: content.id,
      });
      dialogRef.afterClosed().subscribe(async (result) => {
        if (result) await this.uploadVariant(result);
      });
    }
  }

  async uploadVariant(resultData: AddVariantResultData) {
    const content = this.content();
    if (
      content !== undefined &&
      content !== null &&
      typeof content.id === 'string'
    ) {
      let id = content.id;
      if (
        content.variants !== null &&
        content.variants !== undefined &&
        content.variants.find((e) => {
          return e.name == resultData.variantName;
        }) !== undefined
      ) {
        const alreadExistsMessage = await this.translate
          .get(TranslationConstants.VARIANT_OVERVIEW.ALREADY_EXISTS_MESSAGE)
          .toAsync();
        this.moryxSnackbar.showError(alreadExistsMessage);
      } else {
        this.mediaService
          .uploadVariant(id, resultData.variantName, resultData.file)
          .subscribe({
            next: (data) => {
              this.mediaService.loadContent(id).subscribe((x) => {
                if (x !== undefined) {
                  //waits 0.8s to give the server time to create the preview
                  this.mediaService.wait(800).then(() => {
                    this.addPreview(
                      resultData.variantName,
                      resultData.file.type,
                      id
                    );
                    this.content.update(_ => x);
                  });
                }
              });
            },
            error: async (e: HttpErrorResponse) =>
              await this.moryxSnackbar.handleError(e),
          });
      }
    }
  }

  //downloads currently selected variant to PC
  onDownload(variant: VariantDescriptor) {
    const content = this.content();
    const selectedVariant = this.selectedVariant();
    if (
      content !== undefined &&
      selectedVariant !== undefined &&
      typeof this.downloadPictureUrl === 'string'
    ) {
      const a = document.createElement('a');
      a.setAttribute('style', 'display:none;');
      document.body.appendChild(a);
      a.download = content.name + '_' + selectedVariant.name;
      a.href = this.downloadPictureUrl;
      a.target = '_blank';
      a.click();
      document.body.removeChild(a);
    }
  }

  interpolateUrl = (string: string, values: any) =>
    string.replace(/{(.*?)}/g, (match, offset) => values[offset]);

  onInfo(variant: VariantDescriptor): void {
    const content = this.content();
    const selectedVariant = this.selectedVariant();
    if (selectedVariant !== undefined && content !== undefined) {
      const values = { guid: content.id, variantName: variant.name };
      const url =
        location.origin +
        this.interpolateUrl(MediaServerService.GetVariantStreamPath, values);
      const dialogRef = this.dialog.open(DialogVariantInfoComponent, {
        data: {
          name: selectedVariant.name,
          contentName: content.name,
          contentId: content.id,
          creationDate: selectedVariant.creationDate,
          size: selectedVariant.size,
          url: url,
        },
      });
    }
  }

  async onDelete(variant: VariantDescriptor): Promise<void> {
    const content = this.content();
    const selectedVariant = this.selectedVariant();
    if (
      selectedVariant !== undefined &&
      typeof selectedVariant.name === 'string' &&
      selectedVariant.name?.localeCompare('master') !== 0
    ) {
      const deleteMessage = await this.translate
        .get(TranslationConstants.VARIANT_OVERVIEW.DELETE_MESSAGE)
        .toAsync();
      const dialogRef = this.dialog.open(DialogDeleteComponent, {
        data: {
          type: 'Content',
          deleteMessage: deleteMessage,
        },
      });

      dialogRef.afterClosed().subscribe((result) => {
        if (result === true) {
          if (
            selectedVariant !== undefined &&
            content !== undefined &&
            typeof content.id === 'string' &&
            typeof selectedVariant.name === 'string'
          ) {
            this.mediaService
              .removeVariant(content.id, selectedVariant.name)
              .subscribe((response) => {
                if (
                  selectedVariant !== undefined &&
                  typeof selectedVariant.name === 'string'
                ) {
                  this.remove(selectedVariant);
                  this.previews.update(map =>{
                    map.delete(selectedVariant.name!);
                    return map;
                  })
                  this.bigPictureUrl.update(_ => '');
                  this.bigPictureLoadingState.update(_ => 0);
                }
              });
          }
        }
      });
    }
  }

  remove(variant: VariantDescriptor) {
    const content = this.content();
    if (
      content !== undefined &&
      content.variants !== undefined &&
      content.variants !== null
    ) {
      var index = content.variants.findIndex(
        (c) => c.name === variant.name
      );
      if (index > -1) {
        content.variants.splice(index, 1);
      }
    }
  }
}
