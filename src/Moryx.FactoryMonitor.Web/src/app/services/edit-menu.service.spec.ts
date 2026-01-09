import { TestBed } from '@angular/core/testing';

import { EditMenuService } from './edit-menu.service';

describe('EditMenuService', () => {
  let service: EditMenuService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EditMenuService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
