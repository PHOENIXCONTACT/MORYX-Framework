import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditMenuComponent } from './edit-menu.component';

describe('EditMenuComponent', () => {
  let component: EditMenuComponent;
  let fixture: ComponentFixture<EditMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [EditMenuComponent]
})
    .compileComponents();

    fixture = TestBed.createComponent(EditMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
