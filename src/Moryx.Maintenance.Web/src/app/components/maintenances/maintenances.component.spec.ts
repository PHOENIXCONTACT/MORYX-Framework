import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MaintenancesComponent } from './maintenances.component';

describe('MaintenancesComponent', () => {
  let component: MaintenancesComponent;
  let fixture: ComponentFixture<MaintenancesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MaintenancesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MaintenancesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
