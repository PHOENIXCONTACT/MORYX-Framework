import { TestBed } from '@angular/core/testing';

import { FactoryStateStreamService } from './factory-state-stream.service';

describe('FactoryStateStreamService', () => {
  let service: FactoryStateStreamService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FactoryStateStreamService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
