import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteAssignmentComponent } from './delete-assignment.component';

describe('DeleteAssignmentComponent', () => {
  let component: DeleteAssignmentComponent;
  let fixture: ComponentFixture<DeleteAssignmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DeleteAssignmentComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DeleteAssignmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
