import { TestBed } from '@angular/core/testing';

import { LauncherStateService } from './launcher-state.service';

describe('LauncherStateService', () => {
  let service: LauncherStateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LauncherStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
