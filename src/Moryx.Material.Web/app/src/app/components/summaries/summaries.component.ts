import { Component } from '@angular/core';
import { SummaryComponent } from "../summary/summary.component";

@Component({
  selector: 'app-summaries',
  imports: [SummaryComponent],
  templateUrl: './summaries.component.html',
  styleUrl: './summaries.component.scss',
})
export class SummariesComponent {

}
