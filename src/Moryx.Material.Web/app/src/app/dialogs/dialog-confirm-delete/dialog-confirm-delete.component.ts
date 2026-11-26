import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-dialog-confirm-delete',
  imports: [MatDialogModule, MatButtonModule, TranslatePipe],
  templateUrl: './dialog-confirm-delete.component.html',
  styleUrl: './dialog-confirm-delete.component.scss',
})
export class DialogConfirmDeleteComponent {
 protected translationConstants = TranslationConstants;
}
