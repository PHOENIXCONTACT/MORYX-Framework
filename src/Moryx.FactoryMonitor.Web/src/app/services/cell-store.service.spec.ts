import { TestBed } from '@angular/core/testing';
import { CellStoreService } from './cell-store.service';


describe('CellStoreServiceService', () => {
  let service: CellStoreService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CellStoreService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
