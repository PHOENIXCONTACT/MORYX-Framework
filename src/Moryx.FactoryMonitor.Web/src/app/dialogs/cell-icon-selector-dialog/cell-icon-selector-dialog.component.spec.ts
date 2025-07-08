import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CellIconUploaderDialogComponent } from './cell-icon-selector-dialog.component';

describe('CellIconUploaderDialogComponent', () => {
  let component: CellIconUploaderDialogComponent;
  let fixture: ComponentFixture<CellIconUploaderDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [CellIconUploaderDialogComponent]
})
    .compileComponents();

    fixture = TestBed.createComponent(CellIconUploaderDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
