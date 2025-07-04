import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductReferencesComponent } from './product-references.component';

describe('ProductReferencesComponent', () => {
  let component: ProductReferencesComponent;
  let fixture: ComponentFixture<ProductReferencesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductReferencesComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductReferencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
