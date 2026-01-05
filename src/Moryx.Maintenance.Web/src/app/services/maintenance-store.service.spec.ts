import { TestBed } from '@angular/core/testing';

import { MaintenanceStoreService } from './maintenance-store.service';

describe('MaintenanceStoreService', () => {
  let service: MaintenanceStoreService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MaintenanceStoreService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
