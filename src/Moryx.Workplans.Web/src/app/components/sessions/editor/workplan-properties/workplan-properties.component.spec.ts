import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkplanPropertiesComponent } from './workplan-properties.component';

describe('WorkplanPropertiesComponent', () => {
  let component: WorkplanPropertiesComponent;
  let fixture: ComponentFixture<WorkplanPropertiesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [WorkplanPropertiesComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkplanPropertiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
