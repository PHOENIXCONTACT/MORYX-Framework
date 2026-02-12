/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, input } from '@angular/core';
import { Entry, NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';

@Component({
  selector: 'app-operation-source',
  templateUrl: './operation-source.html',
  styleUrls: ['./operation-source.scss'],
  standalone: true,
  imports: [
    NavigableEntryEditor
]
})
export class OperationSource {
  operationSource = input.required<Entry>();

  constructor(
  ) {}
}

