import { Component, Inject, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ResourceModel } from '../../api/models';

@Component({
    selector: 'app-dialog-remove-resource',
    templateUrl: './dialog-remove-resource.component.html',
    styleUrls: ['./dialog-remove-resource.component.scss'],
    standalone: true,
    imports: [
        CommonModule,
        TranslateModule,
        MatDialogModule,
        MatButtonModule
    ]
})
export class DialogRemoveResourceComponent {
    resourceToBeRemoved = signal<ResourceModel | undefined>(undefined);
    TranslationConstants = TranslationConstants;

    constructor(
        public dialogRef: MatDialogRef<DialogRemoveResourceComponent>,
        @Inject(MAT_DIALOG_DATA) public data: ResourceModel,
        public translate: TranslateService,
    ) {
        this.resourceToBeRemoved.update(_ => data);
    }

    onClose() {
        this.dialogRef.close();
    }
}
