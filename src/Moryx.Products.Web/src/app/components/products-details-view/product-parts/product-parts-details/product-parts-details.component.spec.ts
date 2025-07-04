import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProductPartsDetailsComponent } from './product-parts-details.component';

describe('ProductPartsDetailsComponent', () => {
  let component: ProductPartsDetailsComponent;
  let fixture: ComponentFixture<ProductPartsDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ProductPartsDetailsComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductPartsDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
