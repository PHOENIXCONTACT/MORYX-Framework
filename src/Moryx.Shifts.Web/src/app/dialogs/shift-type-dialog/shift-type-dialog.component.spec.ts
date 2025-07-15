import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShiftTypeDialogComponent } from './shift-type-dialog.component';

describe('ShiftTypeDialogComponent', () => {
  let component: ShiftTypeDialogComponent;
  let fixture: ComponentFixture<ShiftTypeDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShiftTypeDialogComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ShiftTypeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
