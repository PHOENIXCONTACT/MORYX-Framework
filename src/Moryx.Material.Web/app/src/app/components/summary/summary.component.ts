import { Component, input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { OrderByInstanceItem, OrderNumber } from '../summaries/summaries.component';

@Component({
  selector: 'app-summary',
  imports: [MatCardModule, MatIconModule],
  templateUrl: './summary.component.html',
  styleUrl: './summary.component.scss',
})
export class SummaryComponent {
}
