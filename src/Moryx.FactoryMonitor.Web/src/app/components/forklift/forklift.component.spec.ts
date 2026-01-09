import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ForkliftComponent } from './forklift.component';

describe('ForkliftComponent', () => {
  let component: ForkliftComponent;
  let fixture: ComponentFixture<ForkliftComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [ForkliftComponent]
})
    .compileComponents();

    fixture = TestBed.createComponent(ForkliftComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
