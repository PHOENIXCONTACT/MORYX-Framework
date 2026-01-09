import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AvailabilitiesComponent } from './availabilities.component';

describe('AttandancesComponent', () => {
  let component: AvailabilitiesComponent;
  let fixture: ComponentFixture<AvailabilitiesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AvailabilitiesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AvailabilitiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
