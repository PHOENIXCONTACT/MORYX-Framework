import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkstationOperatorsComponent } from './workstation-operators.component';

describe('WorkstationOperatorsComponent', () => {
  let component: WorkstationOperatorsComponent;
  let fixture: ComponentFixture<WorkstationOperatorsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkstationOperatorsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WorkstationOperatorsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
