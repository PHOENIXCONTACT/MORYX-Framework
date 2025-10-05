import { TestBed } from '@angular/core/testing';

import { EditResourceService } from './edit-resource.service';

describe('EditResourceService', () => {
  let service: EditResourceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EditResourceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
