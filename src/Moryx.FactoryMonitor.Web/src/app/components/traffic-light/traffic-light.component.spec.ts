import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrafficLightComponent } from './traffic-light.component';

describe('TrafficLightComponent', () => {
  let component: TrafficLightComponent;
  let fixture: ComponentFixture<TrafficLightComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [TrafficLightComponent]
})
    .compileComponents();

    fixture = TestBed.createComponent(TrafficLightComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
