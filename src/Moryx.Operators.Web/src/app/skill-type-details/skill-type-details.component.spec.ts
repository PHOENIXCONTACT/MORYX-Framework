import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SkillTypeDetailsComponent } from './skill-type-details.component';

describe('SkillTypeDetailsComponent', () => {
  let component: SkillTypeDetailsComponent;
  let fixture: ComponentFixture<SkillTypeDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SkillTypeDetailsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SkillTypeDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
