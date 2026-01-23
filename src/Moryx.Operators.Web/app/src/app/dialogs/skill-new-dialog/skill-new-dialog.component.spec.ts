/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SkillNewDialogComponent } from './skill-new-dialog.component';

describe('SkillNewDialogComponent', () => {
  let component: SkillNewDialogComponent;
  let fixture: ComponentFixture<SkillNewDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SkillNewDialogComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SkillNewDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

