import { TestBed } from '@angular/core/testing';

import { ChangeBackgroundService } from './change-background.service';

describe('ChangeBackgroundService', () => {
  let service: ChangeBackgroundService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ChangeBackgroundService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
