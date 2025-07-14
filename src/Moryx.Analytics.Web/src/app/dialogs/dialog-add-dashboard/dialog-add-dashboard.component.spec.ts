import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogAddDashboardComponent } from './dialog-add-dashboard.component';

describe('DialogAddDashboardComponent', () => {
  let component: DialogAddDashboardComponent;
  let fixture: ComponentFixture<DialogAddDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DialogAddDashboardComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogAddDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
