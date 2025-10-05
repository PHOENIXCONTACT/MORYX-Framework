import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogEditDashboardComponent } from './dialog-edit-dashboard.component';

describe('DialogEditDashboardComponent', () => {
  let component: DialogEditDashboardComponent;
  let fixture: ComponentFixture<DialogEditDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DialogEditDashboardComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogEditDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
