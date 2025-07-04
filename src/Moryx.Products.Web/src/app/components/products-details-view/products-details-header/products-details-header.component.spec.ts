import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductsDetailsHeaderComponent } from './products-details-header.component';

describe('ProductsDetailsHeaderComponent', () => {
  let component: ProductsDetailsHeaderComponent;
  let fixture: ComponentFixture<ProductsDetailsHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductsDetailsHeaderComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductsDetailsHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
