import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailsConfigurationDialogComponent } from './details-configuration-dialog.component';

describe('DetailsConfigurationDialogComponent', () => {
  let component: DetailsConfigurationDialogComponent;
  let fixture: ComponentFixture<DetailsConfigurationDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [DetailsConfigurationDialogComponent]
})
    .compileComponents();

    fixture = TestBed.createComponent(DetailsConfigurationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
