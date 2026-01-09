import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceMethodsComponent } from './resource-methods.component';

describe('ResourceMethodsComponent', () => {
  let component: ResourceMethodsComponent;
  let fixture: ComponentFixture<ResourceMethodsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ResourceMethodsComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceMethodsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
