/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Directive, forwardRef, Input } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, Validator } from '@angular/forms';

@Directive({
    selector: '[restrictKeywordValidator][ngModel],[restrictKeywordValidator][formControl],[restrictKeywordValidator][formControlName]',
    providers: [
        { provide: NG_VALIDATORS, useExisting: forwardRef(() => RestrictKeywordValidatorDirective), multi: true }
    ],
    standalone: false
})
// Class definition for Custom Validator
export class RestrictKeywordValidatorDirective implements Validator {
    @Input('restrictKeywordValidator') restrictedKeyword: string | undefined;
    validate(ctrl: AbstractControl): { [key: string]: boolean } | null {
        return ctrl.value === this.restrictedKeyword ? { 'invalidValue': true } : null;
    }
}
