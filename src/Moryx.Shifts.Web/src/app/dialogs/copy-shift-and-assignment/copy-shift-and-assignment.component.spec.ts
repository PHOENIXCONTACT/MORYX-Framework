import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CopyShiftAndAssignmentComponent } from './copy-shift-and-assignment.component';

describe('CopyShiftAndAssignmentComponent', () => {
  let component: CopyShiftAndAssignmentComponent;
  let fixture: ComponentFixture<CopyShiftAndAssignmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CopyShiftAndAssignmentComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(CopyShiftAndAssignmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
