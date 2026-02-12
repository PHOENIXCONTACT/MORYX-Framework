/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, Inject, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';

import { MatButtonModule } from '@angular/material/button';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ResourceModel } from 'src/app/api/models/resource-model';

@Component({
    selector: 'app-dialog-remove-resource',
    templateUrl: './dialog-remove-resource.html',
    styleUrls: ['./dialog-remove-resource.scss'],
    standalone: true,
    imports: [
    TranslateModule,
    MatDialogModule,
    MatButtonModule
]
})
export class DialogRemoveResource {
    resourceToBeRemoved = signal<ResourceModel | undefined>(undefined);
    TranslationConstants = TranslationConstants;

    constructor(
        public dialogRef: MatDialogRef<DialogRemoveResource>,
        @Inject(MAT_DIALOG_DATA) public data: ResourceModel,
        public translate: TranslateService,
    ) {
        this.resourceToBeRemoved.update(_ => data);
    }

    onClose() {
        this.dialogRef.close();
    }
}
