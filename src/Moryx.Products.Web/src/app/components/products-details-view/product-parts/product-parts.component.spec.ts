import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductPartsComponent } from './product-parts.component';

describe('ProductPartsComponent', () => {
  let component: ProductPartsComponent;
  let fixture: ComponentFixture<ProductPartsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductPartsComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductPartsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
