/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatList, MatListModule } from '@angular/material/list';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { EmptyStateComponent, MoryxSnackbarService } from '@moryx/ngx-web-framework';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { NgxDocViewerModule } from 'ngx-doc-viewer';
import { BehaviorSubject } from 'rxjs';
import { DocumentModel } from "../../api/models";
import { OperationModel } from "../../api/models";
import { OrderManagementService } from 'src/app/api/services';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { environment } from 'src/environments/environment';

@Component({
    selector: 'app-operation-documents',
    templateUrl: './operation-documents.component.html',
    styleUrls: ['./operation-documents.component.scss'],
    imports:[
      MatProgressBarModule,
      MatSidenavModule,
      MatToolbarModule,
      CommonModule,
      TranslateModule,
      MatListModule,
      NgxDocViewerModule,
      EmptyStateComponent,
      MatButtonModule,
      RouterLink,
      MatIconModule
    ],
    standalone: true
})
export class OperationDocumentsComponent implements OnInit {
  isLoading = signal<boolean>(false);
  operation = signal<OperationModel>(<OperationModel>{});
  documents = signal<DocumentModel[]>([]);
  selectedDocument = signal<DocumentModel | undefined> (undefined);
  isImage = computed(() => {
    const document = this.selectedDocument()
    return document 
    ? document?.contentType?.includes('image') ?? false
    : false;
  });
  url = signal<string | undefined>(undefined);
  path = signal<string | null | ArrayBuffer>('');

  operationDocumentViewerToolbarImage: string =
    environment.assets + 'assets/operation-document-viewer.jpg';
  TranslationConstants = TranslationConstants;

  constructor(
    private activatedRoute: ActivatedRoute,
    private orderManagementSerivce: OrderManagementService,
    private sanitizer: DomSanitizer,
    public translate: TranslateService,
    private moryxSnackbar: MoryxSnackbarService
  ) {}

  async ngOnInit(): Promise<void> {
    this.isLoading.update( _ => true);
    this.activatedRoute.params.subscribe(async params => {
      let identifier = params['identifier'];
      await this.orderManagementSerivce
        .getOperation({ guid: identifier })
        .toAsync()
        .then(value => (this.operation.update(_=> value)))
        .catch(
          async (e: HttpErrorResponse) =>
            await this.moryxSnackbar.handleError(e)
        );
      this.orderManagementSerivce.getDocuments({ guid: identifier }).subscribe({
        next: data => {
          this.documents.update(_ => data);
          this.isLoading.update( _=> false);
        },
        error: async (e: HttpErrorResponse) =>
          await this.moryxSnackbar.handleError(e),
      });
    });
  }

  async onSelect(document: DocumentModel) {
    this.isLoading.update(_=> true);
    this.selectedDocument.update( _=> document);
    this.orderManagementSerivce
      .getDocumentStream({
        guid: this.operation().identifier!,
        identifier: this.selectedDocument()?.identifier!,
      })
      .subscribe({
        next: data => {
          if (data !== null && document.contentType) {
            let downloadedFile = new Blob([data], {
              type: document.contentType,
            });
            this.url.update(_=> URL.createObjectURL(downloadedFile));
            let reader = new FileReader();
            reader.readAsDataURL(downloadedFile);
            reader.onload = () => {
              this.path.update(_=> reader.result);
            };
            this.isLoading.update(_=> false);
          }
        },
        error: async (e: HttpErrorResponse) =>
          await this.moryxSnackbar.handleError(e),
      });
  }
}


