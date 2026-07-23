import { TestBed } from '@angular/core/testing';

import { MaterialFlowService } from './material-flow.service';

describe('MaterialFlowService', () => {
  let service: MaterialFlowService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MaterialFlowService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
