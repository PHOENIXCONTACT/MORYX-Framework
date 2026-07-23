
import { Component, input } from '@angular/core';
import { MatAnchor, MatButtonModule } from "@angular/material/button";
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import {MatChipsModule} from '@angular/material/chips';
@Component({
  selector: 'app-card',
  imports: [MatAnchor, MatIconModule, MatButtonModule, MatCardModule, MatChipsModule],
  templateUrl: './card.component.html',
  styleUrl: './card.component.scss',
})
export class CardComponent {
  icon = input.required<string>();
  title = input.required<string>();
  instanceCount = input.required<string>();
  links = input<string[]>([]);
}
