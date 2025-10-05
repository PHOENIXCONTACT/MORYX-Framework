import { TestBed } from '@angular/core/testing';

import { BrowserStorageService } from './browser-storage.service';

describe('BrowserStorageService', () => {
  let service: BrowserStorageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BrowserStorageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
