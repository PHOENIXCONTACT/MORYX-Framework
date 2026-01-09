import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CellImageDialogComponent } from './cell-image-dialog.component';

describe('CellImageDialogComponent', () => {
  let component: CellImageDialogComponent;
  let fixture: ComponentFixture<CellImageDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [CellImageDialogComponent]
})
    .compileComponents();

    fixture = TestBed.createComponent(CellImageDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
