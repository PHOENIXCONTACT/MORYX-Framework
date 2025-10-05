import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogRemoveDashboardsComponent } from './dialog-remove-dashboards.component';

describe('DialogRemoveDashboardsComponent', () => {
  let component: DialogRemoveDashboardsComponent;
  let fixture: ComponentFixture<DialogRemoveDashboardsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DialogRemoveDashboardsComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogRemoveDashboardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
