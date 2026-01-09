import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DialogRemoveResourceComponent } from './dialog-remove-resource.component';

describe('DialogRemoveResourceComponent', () => {
    let component: DialogRemoveResourceComponent;
    let fixture: ComponentFixture<DialogRemoveResourceComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [DialogRemoveResourceComponent],
        }).compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(DialogRemoveResourceComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
