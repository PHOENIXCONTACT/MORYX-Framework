import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MediaOverviewComponent } from './media-overview.component';

describe('MediaOverviewComponent', () => {
  let component: MediaOverviewComponent;
  let fixture: ComponentFixture<MediaOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [MediaOverviewComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MediaOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
