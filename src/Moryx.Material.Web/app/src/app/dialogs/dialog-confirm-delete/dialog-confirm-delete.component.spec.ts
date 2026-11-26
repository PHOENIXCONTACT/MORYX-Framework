import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogConfirmDeleteComponent } from './dialog-confirm-delete.component';

describe('DialogConfirmDeleteComponent', () => {
  let component: DialogConfirmDeleteComponent;
  let fixture: ComponentFixture<DialogConfirmDeleteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DialogConfirmDeleteComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DialogConfirmDeleteComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
