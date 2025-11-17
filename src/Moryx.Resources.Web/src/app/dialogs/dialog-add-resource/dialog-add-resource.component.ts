import { Component, inject, model, OnInit, signal } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { MethodEntry, ResourceModel, ResourceTypeModel } from '../../api/models';
import { Permissions } from './../../extensions/permissions.extensions';
import { MatListModule, MatSelectionListChange } from '@angular/material/list';
import { CacheResourceService } from 'src/app/services/cache-resource.service';
import { ResourceConstructionParameters } from 'src/app/models/ResourceConstructionParameters';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { CommonModule } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NavigableEntryEditorComponent } from '@moryx/ngx-web-framework';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-dialog-add-resource',
  templateUrl: './dialog-add-resource.component.html',
  styleUrls: ['./dialog-add-resource.component.scss'],
  imports: [
    TranslateModule,
    CommonModule,
    MatButtonModule,
    FormsModule,
    MatStepperModule,
    MatListModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatDialogModule,
    NavigableEntryEditorComponent,
  ],
  standalone: true,
})
export class DialogAddResourceComponent implements OnInit {
  types = signal<ResourceTypeModel[] | undefined>([]);
  resourceType = signal<ResourceTypeModel | undefined>(undefined);
  selectedCtor = signal<MethodEntry | undefined>(undefined);

  TranslationConstants = TranslationConstants;
  Permissions = Permissions;

  public data = inject<ResourceModel>(MAT_DIALOG_DATA);

  constructor(
    public dialogRef: MatDialogRef<DialogAddResourceComponent>,
    private cache: CacheResourceService,
    public translate: TranslateService
  ) {}

  ngOnInit(): void {
    this.types.update(() => this.cache.flatTypes?.filter(t => t.creatable).sort((a, b) => this.byName(a, b)));
  }

  byName(a: ResourceTypeModel, b: ResourceTypeModel): number {
    return (a.displayName ?? a.name)?.localeCompare(b.displayName ?? b.name ?? '') ?? -1;
  }

  onTypeSelectionChanged(event: MatSelectionListChange) {
    this.resourceType.update(() => event.options[0].value);
    this.selectedCtor.update(() => undefined);
  }

  typeSelected(stepper: MatStepper) {
    if (!this.resourceType()?.constructors?.length) this.skipCtorSelection(stepper);
    stepper.next();
  }

  private skipCtorSelection(stepper: MatStepper) {
    stepper.next();
    if (stepper.selected) stepper.selected.interacted = true;
  }

  secondStepComplete(): boolean {
    return !!(
      this.selectedCtor() ||
      (this.resourceType() && (!this.resourceType()?.constructors || !this.resourceType()?.constructors?.length))
    );
  }

  onCtorSelectionChanged(event: MatSelectionListChange) {
    this.selectedCtor.update(() => event.options[0].value);
  }

  createResult(): ResourceConstructionParameters {
    return {
      name: this.resourceType()?.name,
      method: this.selectedCtor(),
    } as ResourceConstructionParameters;
  }
}
