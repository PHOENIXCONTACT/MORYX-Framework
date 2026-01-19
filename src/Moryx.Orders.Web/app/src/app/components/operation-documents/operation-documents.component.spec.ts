/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperationDocumentsComponent } from './operation-documents.component';

describe('OperationDocumentsComponent', () => {
  let component: OperationDocumentsComponent;
  let fixture: ComponentFixture<OperationDocumentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [OperationDocumentsComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OperationDocumentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

