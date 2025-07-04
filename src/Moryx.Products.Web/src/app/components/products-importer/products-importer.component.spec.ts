import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductsImporterComponent } from './products-importer.component';

describe('ProductsImporterComponent', () => {
  let component: ProductsImporterComponent;
  let fixture: ComponentFixture<ProductsImporterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductsImporterComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductsImporterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
