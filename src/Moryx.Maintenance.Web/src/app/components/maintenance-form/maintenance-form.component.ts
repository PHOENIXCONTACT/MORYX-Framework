import { CommonModule } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { Entry,  NavigableEntryEditorComponent } from '@moryx/ngx-web-framework';

@Component({
  selector: 'app-maintenance-form',
  imports: [
    CommonModule,
    NavigableEntryEditorComponent,
  ],
  templateUrl: './maintenance-form.component.html',
  styleUrl: './maintenance-form.component.scss'
})
export class MaintenanceFormComponent {
  entry = input.required<Entry>();
}
