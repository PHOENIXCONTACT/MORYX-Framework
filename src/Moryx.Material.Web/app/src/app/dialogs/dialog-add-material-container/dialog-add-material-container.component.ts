import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, OnDestroy, OnInit, resource, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule, MatSelectionListChange } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { MethodEntry, NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';
import { firstValueFrom, Observable, SubscriptionLike } from 'rxjs';
import { MaterialContainerTypeModel, ResourceTypeModel } from 'src/app/api/models';
import { MaterialManagementService, ResourceModificationService } from 'src/app/api/services';

@Component({
  selector: 'app-dialog-add-material-container',
  imports: [CommonModule,
    MatButtonModule,
    FormsModule,
    MatStepperModule,
    MatListModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatDialogModule,
    NavigableEntryEditor],
  templateUrl: './dialog-add-material-container.component.html',
  styleUrl: './dialog-add-material-container.component.scss',
})
export class DialogAddMaterialContainerComponent implements OnDestroy{
  types = signal<ResourceTypeModel[]>([]);
  resourceType = signal<ResourceTypeModel | undefined>(undefined);
  selectedCtor = signal<MethodEntry | undefined>(undefined);

  private materialApi = inject(MaterialManagementService);
  private resourceApi = inject(ResourceModificationService);
  private subscriptions: SubscriptionLike[] = [];

  constructor() {
    const sub = this.materialApi.getTypes().subscribe(materialTypes => {
      const promises =  materialTypes.map(x => x.fullName).map(t => firstValueFrom(this.resourceApi.getType({name : t ?? ''})));
      Promise.all(promises).then(types => {
        this.types.set(types);
      })
    })
    this.subscriptions.push(sub);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  byName(a: any, b: any): number {
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

  createResult(): any {
    return {
      name: this.resourceType()?.name,
      method: this.selectedCtor(),
    } as any;
  }
}

