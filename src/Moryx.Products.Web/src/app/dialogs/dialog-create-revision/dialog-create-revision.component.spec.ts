import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogCreateRevisionComponent } from './dialog-create-revision.component';

describe('DialogCreateRevisionComponent', () => {
  let component: DialogCreateRevisionComponent;
  let fixture: ComponentFixture<DialogCreateRevisionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [DialogCreateRevisionComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogCreateRevisionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
