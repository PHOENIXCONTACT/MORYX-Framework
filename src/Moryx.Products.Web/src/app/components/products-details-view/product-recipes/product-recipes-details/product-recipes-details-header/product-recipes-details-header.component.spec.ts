import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductRecipesDetailsHeaderComponent } from './product-recipes-details-header.component';

describe('ProductRecipesDetailsHeaderComponent', () => {
  let component: ProductRecipesDetailsHeaderComponent;
  let fixture: ComponentFixture<ProductRecipesDetailsHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductRecipesDetailsHeaderComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductRecipesDetailsHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
