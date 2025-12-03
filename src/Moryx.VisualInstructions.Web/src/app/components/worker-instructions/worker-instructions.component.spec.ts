import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerInstructionsComponent } from './worker-instructions.component';

describe('WorkerInstructionsComponent', () => {
  let component: WorkerInstructionsComponent;
  let fixture: ComponentFixture<WorkerInstructionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [WorkerInstructionsComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkerInstructionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
