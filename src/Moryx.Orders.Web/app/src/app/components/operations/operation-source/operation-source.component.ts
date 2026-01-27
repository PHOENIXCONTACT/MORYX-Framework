/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/


import { Component, input, Input, OnInit } from '@angular/core';
import { Entry, NavigableEntryEditorComponent } from '@moryx/ngx-web-framework';

@Component({
  selector: 'app-operation-source',
  templateUrl: './operation-source.component.html',
  styleUrls: ['./operation-source.component.scss'],
  standalone: true,
  imports: [
    NavigableEntryEditorComponent
]
})
export class OperationSourceComponent {
  operationSource = input.required<Entry>();
  
  constructor(
  ) {}
}

