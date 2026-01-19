/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { OverlayContainer } from '@angular/cdk/overlay';
import {} from '@angular/common/http';
import {
  ComponentFixture,
  fakeAsync,
  TestBed,
  tick
} from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatDialog,
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { MoryxSnackbarService } from '@moryx/ngx-web-framework';
import { TranslateModule } from '@ngx-translate/core';
import { of } from 'rxjs';
import {
  OperationRecipeModel,
  ProductQuery,
  RecipeClassificationModel,
  RecipeModel
} from 'src/app/api/models';
import {
  OrderManagementService,
  ProductManagementService
} from 'src/app/api/services';

import { CreateDialogComponent } from './create-dialog.component';

describe('CreateDialogComponent', () => {
  let component: CreateDialogComponent;
  let dialog: MatDialog;
  let fixture: ComponentFixture<CreateDialogComponent>;
  let overlayContainerElement: HTMLElement;

  let recipes = [
    <RecipeModel>{
      id: 1,
      name: 'Recipe 1',
      revision: 1,
      classification: RecipeClassificationModel.Alternative,
    },
    <RecipeModel>{
      id: 2,
      name: 'Recipe 2',
      revision: 1,
      classification: RecipeClassificationModel.Default,
    },
    <RecipeModel>{
      id: 3,
      name: 'Recipe 3',
      revision: 1,
      classification: RecipeClassificationModel.Alternative,
    },
  ];

  let orderManagementMock = {
    getAssignableRecipes: function (params?: { identifier?: string; revision?: number }) {
      return of(recipes.map(r => <OperationRecipeModel>{ id: r.id, name: r.name }));
    },
  };

  let productManagementMock = {
    getRecipe: function (params: { id: number }) {
      return of(recipes.find(r => r.id === params.id));
    },

    getTypes: function (params?: { body?: ProductQuery }) {
      return of([]);
    },
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [CreateDialogComponent],
    providers: [
        { provide: OrderManagementService, useValue: orderManagementMock },
        { provide: ProductManagementService, useValue: productManagementMock },
        { provide: MatDialogRef, useValue: {} },
        { provide: MoryxSnackbarService, useValue: {} },
        {
            provide: OverlayContainer,
            useFactory: () => {
                overlayContainerElement = document.createElement('div');
                return { getContainerElement: () => overlayContainerElement };
            },
        },
    ],
    imports: [
        NoopAnimationsModule,
        MatDialogModule,
        MatFormFieldModule,
        MatListModule,
        MatIconModule,
        MatProgressBarModule,
        MatSelectModule,
        MatInputModule,
        FormsModule,
        ReactiveFormsModule,
        TranslateModule.forRoot(),
    ],
}).compileComponents();
  });

  beforeEach(() => {
    dialog = TestBed.inject(MatDialog);
    fixture = TestBed.createComponent(CreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set a default recipe', fakeAsync(() => {
    // TODO: also check the DOM for showing the selected element
    var component = dialog.open(CreateDialogComponent, { data: undefined });
    component.componentInstance.selectedProduct = {};
    tick();
    fixture.detectChanges();

    expect(component.componentInstance.selectedRecipe.name).toBe('Recipe 2');
  }));
});

