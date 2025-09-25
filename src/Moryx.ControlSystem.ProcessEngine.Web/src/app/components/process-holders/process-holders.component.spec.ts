import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WpcListComponent } from './process-holders.component';

describe('WpcListComponent', () => {
  let component: WpcListComponent;
  let fixture: ComponentFixture<WpcListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WpcListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WpcListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
