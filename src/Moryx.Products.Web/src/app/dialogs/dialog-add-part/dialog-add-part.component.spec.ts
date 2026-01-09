import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogAddPartComponent } from './dialog-add-part.component';

describe('DialogAddPartComponent', () => {
  let component: DialogAddPartComponent;
  let fixture: ComponentFixture<DialogAddPartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DialogAddPartComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogAddPartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
