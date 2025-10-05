import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourcePropertiesComponent } from './resource-properties.component';

describe('ResourcePropertiesComponent', () => {
  let component: ResourcePropertiesComponent;
  let fixture: ComponentFixture<ResourcePropertiesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ResourcePropertiesComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourcePropertiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
