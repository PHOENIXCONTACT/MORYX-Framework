import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OperationRecipesComponent } from './operation-recipes.component';

describe('OperationRecipesComponent', () => {
  let component: OperationRecipesComponent;
  let fixture: ComponentFixture<OperationRecipesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [OperationRecipesComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OperationRecipesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  xit('should create', () => {
    expect(component).toBeTruthy();
  });
});
