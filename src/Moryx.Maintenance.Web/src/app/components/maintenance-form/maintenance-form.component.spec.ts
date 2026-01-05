import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MaintenanceFormComponent } from './maintenance-form.component';

describe('MaintenanceFormComponent', () => {
  let component: MaintenanceFormComponent;
  let fixture: ComponentFixture<MaintenanceFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MaintenanceFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MaintenanceFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
