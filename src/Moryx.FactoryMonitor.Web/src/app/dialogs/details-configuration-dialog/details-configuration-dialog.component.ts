import { Component } from '@angular/core';
import {
  MatDialog,
  MatDialogRef,
  MatDialogTitle,
  MatDialogContent,
  MatDialogActions,
  MatDialogClose,
  MatDialogModule,
} from '@angular/material/dialog';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatSelectionList, MatListOption, MatListModule } from '@angular/material/list';
import { CommonModule, NgFor } from '@angular/common';
import { MatButton, MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-details-configuration-dialog',
  templateUrl: './details-configuration-dialog.component.html',
  styleUrls: ['./details-configuration-dialog.component.scss'],
  imports: [
    MatDialogModule,
    CdkScrollable,
    MatListModule,
    CommonModule,
    MatButtonModule,
  ],
  standalone: true
})
export class DetailsConfigurationDialogComponent {
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

  constructor(public propertyConfigurationDialogRef: MatDialogRef<DetailsConfigurationDialogComponent>) {}
}
