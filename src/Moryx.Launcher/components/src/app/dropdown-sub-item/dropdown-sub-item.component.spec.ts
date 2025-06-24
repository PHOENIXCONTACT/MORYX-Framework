import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DropdownSubItemComponent } from './dropdown-sub-item.component';

describe('DropdownSubItemComponent', () => {
  let component: DropdownSubItemComponent;
  let fixture: ComponentFixture<DropdownSubItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DropdownSubItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DropdownSubItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
