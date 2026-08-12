/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, OnInit, signal, ChangeDetectionStrategy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslatePipe } from '@ngx-translate/core';
import { NgxDocViewerModule } from 'ngx-doc-viewer';
import { DocumentModel, OperationModel } from "@api/models";
import { OrderManagementService } from '@api/services/order-management.service';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { DocumentDefaultView } from './document-default-view/document-default-view';
import { DocumentDetailsHeader } from './document-details-header/document-details-header';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-operation-documents',
  templateUrl: './operation-documents.html',
  styleUrls: ['./operation-documents.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatProgressBarModule,
    MatSidenavModule,
    MatToolbarModule,
    CommonModule,
    TranslatePipe,
    MatListModule,
    NgxDocViewerModule,
    MatButtonModule,
    RouterLink,
    MatIconModule,
    DocumentDefaultView,
    DocumentDetailsHeader
  ]
})
export class OperationDocuments implements OnInit {
  protected isLoading = signal<boolean>(false);
  protected operation = signal<OperationModel>(<OperationModel>{});
  protected documents = signal<DocumentModel[]>([]);
  protected selectedDocument = signal<DocumentModel | undefined>(undefined);
  protected isImage = computed(() => {
    const document = this.selectedDocument()
    return document
      ? document?.contentType?.includes('image') ?? false
      : false;
  });
  protected url = signal<string | undefined>(undefined);
  protected path = signal<string | null | ArrayBuffer>('');

  protected operationDocumentViewerToolbarImage: string =
    environment.assets + 'assets/operation-document-viewer.jpg';
  protected TranslationConstants = TranslationConstants;

  private activatedRoute = inject(ActivatedRoute);
  private orderManagementService = inject(OrderManagementService);
  private snackbarService = inject(SnackbarService);

  async ngOnInit(): Promise<void> {
    this.isLoading.set(true);
    this.activatedRoute.params.subscribe(async params => {
      const identifier = params['identifier'];
      await this.orderManagementService
        .getOperation({guid: identifier})
        .then(value => (this.operation.set(value)))
        .catch(
          async (e: HttpErrorResponse) =>
            await this.snackbarService.handleError(e)
        );
      this.orderManagementService.getDocuments({guid: identifier}).then(data => {
        this.documents.set(data);
        this.isLoading.set(false);
      }).catch(async (e: HttpErrorResponse) =>
        await this.snackbarService.handleError(e)
      );
    });
  }

  protected async onSelect(document: DocumentModel) {
    this.isLoading.set(true);
    this.selectedDocument.set(document);
    this.orderManagementService
      .getDocumentStream({
        guid: this.operation().identifier!,
        identifier: this.selectedDocument()?.identifier ?? ''
      })
      .then(data => {
        if (data !== null && document.contentType) {
          const downloadedFile = new Blob([data], {
            type: document.contentType
          });
          this.url.set(URL.createObjectURL(downloadedFile));
          const reader = new FileReader();
          reader.readAsDataURL(downloadedFile);
          reader.onload = () => {
            this.path.set(reader.result);
          };
          this.isLoading.set(false);
        }
      })
      .catch(async (e: HttpErrorResponse) =>
        await this.snackbarService.handleError(e)
      );
  }
}


