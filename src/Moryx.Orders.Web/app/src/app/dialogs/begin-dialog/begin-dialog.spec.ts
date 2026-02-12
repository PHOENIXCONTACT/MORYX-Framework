/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { ComponentFixture, TestBed } from '@angular/core/testing';
import {
  MatDialogModule,
  MatDialogRef,
  MAT_DIALOG_DATA
} from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatInputModule } from '@angular/material/input';
import { By } from '@angular/platform-browser';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { TranslateModule } from '@ngx-translate/core';

import { FormsModule } from '@angular/forms';
import {
  BeginDialog,
  BeginDialogData
} from './begin-dialog';

class DialogRefMock {
  close(result: any) {}
}

describe('BeginDialog', () => {
  let component: BeginDialog;
  let fixture: ComponentFixture<BeginDialog>;
  let dialogData = <BeginDialogData>{ context: {}, operation: { model: {} } };
  let dialogRef: MatDialogRef<BeginDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    declarations: [BeginDialog],
    providers: [
        { provide: MatDialogRef, useClass: DialogRefMock },
        { provide: MAT_DIALOG_DATA, useValue: dialogData },
    ],
    imports: [
        NoopAnimationsModule,
        MatDialogModule,
        MatGridListModule,
        MatFormFieldModule,
        MatInputModule,
        FormsModule,
        TranslateModule.forRoot(),
    ],
}).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BeginDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  beforeEach(() => {
    dialogRef = component.dialog;
    dialogData.context.canBegin = true;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should return 0 when confirmed', done => {
    const dialogCloseSpy = spyOn(dialogRef, 'close');

    fixture.detectChanges();
    fixture.whenStable().then(() => {
      var submitButton = fixture.debugElement
        .queryAll(By.css('button'))
        .find(
          element =>
            element.nativeElement.textContent === ' BEGIN_DIALOG.BEGIN '
        );

      (submitButton?.nativeElement as HTMLElement).click();
      fixture.detectChanges();

      fixture.whenStable().then(() => {
        fixture.detectChanges();
        expect(dialogCloseSpy).toHaveBeenCalledWith(0);
        done();
      });
    });
  });

  it('should return -1 when canceled', done => {
    const dialogCloseSpy = spyOn(dialogRef, 'close');

    var cancelButton = fixture.debugElement
      .queryAll(By.css('button'))
      .find(
        element => element.nativeElement.textContent === ' BEGIN_DIALOG.CANCEL '
      );
    (cancelButton?.nativeElement as HTMLElement).click();
    fixture.detectChanges();

    fixture.whenStable().then(() => {
      fixture.detectChanges();
      expect(dialogCloseSpy).toHaveBeenCalledWith(-1);
      done();
    });
  });
});

