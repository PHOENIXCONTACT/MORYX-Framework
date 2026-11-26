import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SummariesComponent } from './summaries.component';

describe('SummariesComponent', () => {
  let component: SummariesComponent;
  let fixture: ComponentFixture<SummariesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SummariesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SummariesComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
