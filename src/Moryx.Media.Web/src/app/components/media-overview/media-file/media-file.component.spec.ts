import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MediaFileComponent } from './media-file.component';

describe('MediaFileComponent', () => {
  let component: MediaFileComponent;
  let fixture: ComponentFixture<MediaFileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [MediaFileComponent],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MediaFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
