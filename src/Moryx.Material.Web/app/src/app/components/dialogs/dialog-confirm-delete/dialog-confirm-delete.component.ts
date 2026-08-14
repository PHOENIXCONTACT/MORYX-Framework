import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-dialog-confirm-delete',
  imports: [MatDialogModule, MatButtonModule],
  templateUrl: './dialog-confirm-delete.component.html',
  styleUrl: './dialog-confirm-delete.component.scss',
})
export class DialogConfirmDeleteComponent {

}
