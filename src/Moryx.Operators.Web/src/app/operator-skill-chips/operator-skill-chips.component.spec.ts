/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperatorSkillChipsComponent } from './operator-skill-chips.component';

describe('OperatorSkillChipsComponent', () => {
  let component: OperatorSkillChipsComponent;
  let fixture: ComponentFixture<OperatorSkillChipsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OperatorSkillChipsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(OperatorSkillChipsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

