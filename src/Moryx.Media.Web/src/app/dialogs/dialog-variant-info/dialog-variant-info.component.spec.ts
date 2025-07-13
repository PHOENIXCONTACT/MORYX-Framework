import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogVariantInfoComponent } from './dialog-variant-info.component';

describe('DialogVariantInfoComponent', () => {
  let component: DialogVariantInfoComponent;
  let fixture: ComponentFixture<DialogVariantInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [DialogVariantInfoComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogVariantInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
