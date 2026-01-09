import { TestBed } from '@angular/core/testing';

import { ImporterGuard } from './importer.guard';

describe('ImporterGuard', () => {
  let guard: ImporterGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ImporterGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
