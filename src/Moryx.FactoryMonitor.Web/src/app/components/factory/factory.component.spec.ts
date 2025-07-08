import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FactoryComponent } from './factory.component';

describe('FactoryComponent', () => {
  let component: FactoryComponent;
  let fixture: ComponentFixture<FactoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: []
})
    .compileComponents();

    fixture = TestBed.createComponent(FactoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
