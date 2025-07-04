import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductRecipesComponent } from './product-recipes.component';

describe('ProductRecipesComponent', () => {
  let component: ProductRecipesComponent;
  let fixture: ComponentFixture<ProductRecipesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductRecipesComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductRecipesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
