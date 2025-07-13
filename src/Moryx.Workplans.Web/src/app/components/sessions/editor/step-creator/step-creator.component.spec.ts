import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepCreatorComponent } from './step-creator.component';

describe('StepCreatorComponent', () => {
  let component: StepCreatorComponent;
  let fixture: ComponentFixture<StepCreatorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [StepCreatorComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StepCreatorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
