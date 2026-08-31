import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';

@Component({
  selector: 'app-dialog-confirm-delete',
  imports: [MatDialogModule, MatButtonModule, TranslateModule],
  templateUrl: './dialog-confirm-delete.component.html',
  styleUrl: './dialog-confirm-delete.component.scss',
})
export class DialogConfirmDeleteComponent {
 protected translationConstants = TranslationConstants;
}
