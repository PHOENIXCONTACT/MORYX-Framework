import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WeekDayToggleButtonComponent } from './week-day-toggle-button.component';

describe('WeekDayToggleButtonComponent', () => {
  let component: WeekDayToggleButtonComponent;
  let fixture: ComponentFixture<WeekDayToggleButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WeekDayToggleButtonComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(WeekDayToggleButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
