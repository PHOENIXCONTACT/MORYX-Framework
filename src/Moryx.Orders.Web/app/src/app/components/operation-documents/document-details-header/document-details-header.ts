/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, input, ChangeDetectionStrategy } from '@angular/core';
import { DocumentModel } from '@api/models';

@Component({
  selector: 'app-document-details-header',
  templateUrl: './document-details-header.html',
  styleUrls: ['./document-details-header.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DocumentDetailsHeader {
  readonly selectedDocument = input.required<DocumentModel>();
}
