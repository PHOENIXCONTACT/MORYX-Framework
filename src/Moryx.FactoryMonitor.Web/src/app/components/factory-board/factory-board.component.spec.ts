/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FactoryBoardComponent } from './factory-board.component';

describe('FactoryBoardComponent', () => {
  let component: FactoryBoardComponent;
  let fixture: ComponentFixture<FactoryBoardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FactoryBoardComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(FactoryBoardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

