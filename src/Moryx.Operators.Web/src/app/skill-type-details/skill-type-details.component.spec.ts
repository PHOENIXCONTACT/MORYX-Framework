/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SkillTypeDetailsComponent } from './skill-type-details.component';

describe('SkillTypeDetailsComponent', () => {
  let component: SkillTypeDetailsComponent;
  let fixture: ComponentFixture<SkillTypeDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SkillTypeDetailsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SkillTypeDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

