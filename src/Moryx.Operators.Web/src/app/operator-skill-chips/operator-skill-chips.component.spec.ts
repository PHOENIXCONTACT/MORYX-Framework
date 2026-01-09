import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperatorSkillChipsComponent } from './operator-skill-chips.component';

describe('OperatorSkillChipsComponent', () => {
  let component: OperatorSkillChipsComponent;
  let fixture: ComponentFixture<OperatorSkillChipsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OperatorSkillChipsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(OperatorSkillChipsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
