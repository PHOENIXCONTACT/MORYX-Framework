import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductRecipesDetailsComponent } from './product-recipes-details.component';

describe('ProductRecipesDetailsComponent', () => {
  let component: ProductRecipesDetailsComponent;
  let fixture: ComponentFixture<ProductRecipesDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductRecipesDetailsComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductRecipesDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
