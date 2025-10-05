import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShiftInstanceDialogComponent } from './shift-instance-dialog.component';

describe('ShiftInstanceDialogComponent', () => {
  let component: ShiftInstanceDialogComponent;
  let fixture: ComponentFixture<ShiftInstanceDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShiftInstanceDialogComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ShiftInstanceDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
