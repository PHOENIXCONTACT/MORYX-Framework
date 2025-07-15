import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkHoursIconComponent } from './work-hours-icon.component';

describe('WorkHoursIconComponent', () => {
  let component: WorkHoursIconComponent;
  let fixture: ComponentFixture<WorkHoursIconComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkHoursIconComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WorkHoursIconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
