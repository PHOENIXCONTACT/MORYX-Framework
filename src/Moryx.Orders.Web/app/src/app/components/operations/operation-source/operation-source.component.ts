/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, input } from '@angular/core';
import { Entry, NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';

@Component({
  selector: 'app-operation-source',
  templateUrl: './operation-source.component.html',
  styleUrls: ['./operation-source.component.scss'],
  standalone: true,
  imports: [
    NavigableEntryEditor
]
})
export class OperationSourceComponent {
  operationSource = input.required<Entry>();

  constructor(
  ) {}
}

