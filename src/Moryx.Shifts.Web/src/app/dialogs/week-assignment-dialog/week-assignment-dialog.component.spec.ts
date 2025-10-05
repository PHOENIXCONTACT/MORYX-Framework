import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WeekAssignmentDialogComponent } from './week-assignment-dialog.component';

describe('WeekAssignmentDialogComponent', () => {
  let component: WeekAssignmentDialogComponent;
  let fixture: ComponentFixture<WeekAssignmentDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WeekAssignmentDialogComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WeekAssignmentDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
