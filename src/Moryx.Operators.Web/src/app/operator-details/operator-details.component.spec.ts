import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperatorDetailsComponent } from './operator-details.component';

describe('OperatorDetailsComponent', () => {
  let component: OperatorDetailsComponent;
  let fixture: ComponentFixture<OperatorDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OperatorDetailsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(OperatorDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
