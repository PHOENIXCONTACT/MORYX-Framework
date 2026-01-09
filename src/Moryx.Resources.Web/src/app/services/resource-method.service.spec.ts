import { TestBed } from '@angular/core/testing';

import { ResourceMethodService } from './resource-method.service';

describe('ResourceMethodService', () => {
  let service: ResourceMethodService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ResourceMethodService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
