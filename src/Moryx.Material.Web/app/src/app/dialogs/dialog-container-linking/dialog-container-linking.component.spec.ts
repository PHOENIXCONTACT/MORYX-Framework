import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogContainerLinkingComponent } from './dialog-container-linking.component';

describe('DialogContainerLinkingComponent', () => {
  let component: DialogContainerLinkingComponent;
  let fixture: ComponentFixture<DialogContainerLinkingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DialogContainerLinkingComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DialogContainerLinkingComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
