import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SkillEditDialogComponent } from './skill-edit-dialog.component';

describe('SkillEditDialogComponent', () => {
  let component: SkillEditDialogComponent;
  let fixture: ComponentFixture<SkillEditDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SkillEditDialogComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SkillEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
