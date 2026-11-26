import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';

@Component({
  selector: 'app-dialog-container-linking',
  imports: [MatDialogModule, MatSelectModule, MatInputModule, NavigableEntryEditor, MatButtonModule],
  templateUrl: './dialog-container-linking.component.html',
  styleUrl: './dialog-container-linking.component.scss',
})
export class DialogContainerLinkingComponent {

}
