import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SkillTypesComponent } from './skill-types.component';

describe('SkillTypesComponent', () => {
  let component: SkillTypesComponent;
  let fixture: ComponentFixture<SkillTypesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SkillTypesComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SkillTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
