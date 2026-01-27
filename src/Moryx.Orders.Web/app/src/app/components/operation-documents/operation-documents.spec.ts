/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperationDocuments } from './operation-documents';

describe('OperationDocuments', () => {
  let component: OperationDocuments;
  let fixture: ComponentFixture<OperationDocuments>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [OperationDocuments]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OperationDocuments);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});

