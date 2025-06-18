import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { Entry, NavigableEntryEditorComponent } from '@moryx/ngx-web-framework';
import { Subscription } from 'rxjs';
import { EditResourceService } from '../../../services/edit-resource.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-resource-properties',
  templateUrl: './resource-properties.component.html',
  styleUrls: ['./resource-properties.component.scss'],
  imports: [MatProgressSpinnerModule, NavigableEntryEditorComponent, CommonModule],
  standalone: true,
})
export class ResourcePropertiesComponent implements OnInit, OnDestroy {
  properties = signal<Entry | undefined>(undefined);
  private editServiceSubscription: Subscription | undefined;

  constructor(public editService: EditResourceService) {}

  ngOnInit(): void {
    this.editServiceSubscription = this.editService.activeResource$.subscribe(resource => {
      if (resource?.properties) {
        this.properties.update(() => resource.properties);
      }
    });
  }

  ngOnDestroy(): void {
    this.editServiceSubscription?.unsubscribe();
  }
}
