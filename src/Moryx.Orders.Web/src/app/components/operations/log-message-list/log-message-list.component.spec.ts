import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogMessageListComponent } from './log-message-list.component';

describe('LogMessageDialogComponent', () => {
  let component: LogMessageListComponent;
  let fixture: ComponentFixture<LogMessageListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [LogMessageListComponent],
    providers: [
        {}
    ]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LogMessageListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});
