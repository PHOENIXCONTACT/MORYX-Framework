/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component } from '@angular/core';
import {
  MatDialogRef,
  MatDialogModule,
} from '@angular/material/dialog';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatListModule } from '@angular/material/list';

import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-details-configuration-dialog',
  templateUrl: './details-configuration-dialog.html',
  styleUrls: ['./details-configuration-dialog.scss'],
  imports: [
    MatDialogModule,
    CdkScrollable,
    MatListModule,
    MatButtonModule
],
  standalone: true
})
export class DetailsConfigurationDialog {
  propertyNames: string[] = [
    'Boots',
    'Clogs',
    'Loafers',
    'Moccasins',
    'Sneakers',
    'Boots',
    'Clogs',
    'Loafers',
    'Moccasins',
    'Sneakers',
    'Boots',
    'Clogs',
    'Loafers',
    'Moccasins',
    'Sneakers',
  ];

  constructor(public propertyConfigurationDialogRef: MatDialogRef<DetailsConfigurationDialog>) {}
}

