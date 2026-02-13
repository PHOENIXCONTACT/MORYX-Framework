/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteAssignment } from './delete-assignment';

describe('DeleteAssignment', () => {
  let component: DeleteAssignment;
  let fixture: ComponentFixture<DeleteAssignment>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteAssignment]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DeleteAssignment);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

