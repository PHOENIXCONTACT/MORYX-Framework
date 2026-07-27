import { CommonModule } from '@angular/common';
import { Component, effect, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule, MatSelectionListChange } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { MethodEntry, NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';
import { MaterialContainerTypeModel } from 'src/app/api/models';
import { MaterialManagementService } from 'src/app/api/services';
import { MaterialFlowService } from 'src/app/services/material-flow.service';

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
export class DialogAddMaterialContainerComponent {
  types = signal<MaterialContainerTypeModel[] | undefined>([]);
  resourceType = signal<any | undefined>(undefined);
  selectedCtor = signal<MethodEntry | undefined>(undefined);

  private data = inject<any>(MAT_DIALOG_DATA);
  private dialogRef = inject(MatDialogRef<DialogAddMaterialContainerComponent>);
  private materialApi = inject(MaterialManagementService);
  private typesSource = toSignal(this.materialApi.getTypes())

  constructor(){
    effect(() => {
      const types = this.typesSource();
      this.types.set(types ?? []);
    })
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
