import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AvailabilityDialogComponent } from './availability-dialog.component';

describe('AvailabilityDialogComponent', () => {
  let component: AvailabilityDialogComponent;
  let fixture: ComponentFixture<AvailabilityDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AvailabilityDialogComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AvailabilityDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
