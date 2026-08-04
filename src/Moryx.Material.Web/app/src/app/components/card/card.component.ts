
import { Component, inject, Input, input } from '@angular/core';
import { MatAnchor, MatButtonModule } from "@angular/material/button";
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { DialogPreAdviceComponent } from 'src/app/dialogs/dialog-pre-advice/dialog-pre-advice.component';
import { MaterialContainerModel, ResourceModel, ResourceTypeModel } from 'src/app/api/models';
import { DialogContainerLinkingComponent } from 'src/app/dialogs/dialog-container-linking/dialog-container-linking.component';
@Component({
  selector: 'app-card',
  imports: [MatAnchor, MatIconModule, MatButtonModule, MatCardModule, MatChipsModule],
  templateUrl: './card.component.html',
  styleUrl: './card.component.scss',
})
export class CardComponent {
  links = input<string[]>([]);
  container = input.required<MaterialContainerModel>();
  private dialog = inject(MatDialog);


  preAdvice() {
    this.dialog.open(DialogPreAdviceComponent, {
      data: this.container()
    });
  }

  link() {
    this.dialog.open(DialogContainerLinkingComponent);
  }
}
