import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddOperatorComponentDialog } from './add-operator.component';

describe('AddOperatorComponent', () => {
  let component: AddOperatorComponentDialog;
  let fixture: ComponentFixture<AddOperatorComponentDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddOperatorComponentDialog]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AddOperatorComponentDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
