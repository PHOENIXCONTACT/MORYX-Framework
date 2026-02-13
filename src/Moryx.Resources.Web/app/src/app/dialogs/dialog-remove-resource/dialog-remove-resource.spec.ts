/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DialogRemoveResource } from './dialog-remove-resource';

describe('DialogRemoveResource', () => {
    let component: DialogRemoveResource;
    let fixture: ComponentFixture<DialogRemoveResource>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [DialogRemoveResource],
        }).compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(DialogRemoveResource);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
