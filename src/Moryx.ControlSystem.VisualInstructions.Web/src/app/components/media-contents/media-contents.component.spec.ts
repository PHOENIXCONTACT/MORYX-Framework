import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MediaContentsComponent } from './media-contents.component';

describe('MediaContentsComponent', () => {
  let component: MediaContentsComponent;
  let fixture: ComponentFixture<MediaContentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [MediaContentsComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MediaContentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
