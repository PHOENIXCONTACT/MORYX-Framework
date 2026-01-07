import { TestBed } from '@angular/core/testing';

import { MaintenanceStreamService } from './maintenance-stream.service';

describe('MaintenanceStreamService', () => {
  let service: MaintenanceStreamService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MaintenanceStreamService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
