import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogAddVariantComponent } from './dialog-add-variant.component';

describe('DialogAddVariantComponent', () => {
  let component: DialogAddVariantComponent;
  let fixture: ComponentFixture<DialogAddVariantComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [DialogAddVariantComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogAddVariantComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
