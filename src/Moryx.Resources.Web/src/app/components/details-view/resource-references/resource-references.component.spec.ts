import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceReferencesComponent } from './resource-references.component';

describe('ResourceReferencesComponent', () => {
  let component: ResourceReferencesComponent;
  let fixture: ComponentFixture<ResourceReferencesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [ResourceReferencesComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceReferencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
