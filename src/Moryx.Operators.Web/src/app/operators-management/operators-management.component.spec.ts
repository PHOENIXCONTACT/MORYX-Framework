import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperatorsManagementComponent } from './operators-management.component';

describe('OperatorsManagementComponent', () => {
  let component: OperatorsManagementComponent;
  let fixture: ComponentFixture<OperatorsManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OperatorsManagementComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(OperatorsManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
