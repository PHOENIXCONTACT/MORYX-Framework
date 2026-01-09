import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DefaultDetailsViewComponent } from './default-details-view.component';

describe('DefaultDetailsViewComponent', () => {
  let component: DefaultDetailsViewComponent;
  let fixture: ComponentFixture<DefaultDetailsViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DefaultDetailsViewComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DefaultDetailsViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
