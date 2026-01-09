import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeBackgroundDialogComponent } from './change-background-dialog.component';

describe('ChangeBackgroundDialogComponent', () => {
  let component: ChangeBackgroundDialogComponent;
  let fixture: ComponentFixture<ChangeBackgroundDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [ChangeBackgroundDialogComponent]
})
    .compileComponents();

    fixture = TestBed.createComponent(ChangeBackgroundDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
