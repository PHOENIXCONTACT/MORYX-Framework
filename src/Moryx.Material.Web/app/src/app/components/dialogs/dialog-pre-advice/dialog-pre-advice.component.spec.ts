import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogPreAdviceComponent } from './dialog-pre-advice.component';

describe('DialogPreAdviceComponent', () => {
  let component: DialogPreAdviceComponent;
  let fixture: ComponentFixture<DialogPreAdviceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DialogPreAdviceComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DialogPreAdviceComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
