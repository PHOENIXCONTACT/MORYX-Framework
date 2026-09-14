import { CommonModule } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule, MatSelectionListChange } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { Entry, MethodEntry, NavigableEntryEditor } from '@moryx/ngx-web-framework/entry-editor';
import { firstValueFrom, Observable, SubscriptionLike } from 'rxjs';
import { CdkStepLabel } from "@angular/cdk/stepper";
import { TranslatePipe } from '@ngx-translate/core';
import { ResourceTypeModel } from '@app/api/models';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { MaterialManagementService, ResourceModificationService } from '@app/api/services';
import { Component, inject, OnDestroy, signal } from '@angular/core';

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
    NavigableEntryEditor,
    TranslatePipe,
    CdkStepLabel],
  templateUrl: './dialog-add-material-container.component.html',
  styleUrl: './dialog-add-material-container.component.scss',
})
export class DialogAddMaterialContainerComponent implements OnDestroy {

  types = signal<ResourceTypeModel[]>([]);
  resourceType = signal<ResourceTypeModel | undefined>(undefined);
  selectedCtor = signal<MethodEntry | undefined>(undefined);
  protected translationConstants = TranslationConstants;
  private materialApi = inject(MaterialManagementService);
  private resourceApi = inject(ResourceModificationService);
  private subscriptions: SubscriptionLike[] = [];
  private parametersBusy = signal(false);

  constructor() {
    const sub = this.materialApi.getTypes().subscribe(materialTypes => {
      const promises = materialTypes.map(x => x.fullName).map(t => firstValueFrom(this.resourceApi.getType({ name: t ?? '' })));
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
    if (this.resourceType()?.constructors?.length === 1) {
      this.selectedCtor.update(() => this.resourceType()?.constructors?.at(0)! as MethodEntry);
      this.skipCtorSelection(stepper);
    }
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

  paramsChanged(entry: Entry, method: MethodEntry, editor: NavigableEntryEditor) {
    if (!this.parametersBusy()) {
      this.parametersBusy.set(true);
    } else {
      return;
    }

    this.materialApi.updateMethodParams({
      type: this.resourceType()?.name!,
      body: method
    }).subscribe({
      next: value => {
        this.selectedCtor.update(old => {
          if (old) {
            old.parameters = { ...value.parameters! };
          }
          this.parametersBusy.set(false);
          return old;
        });
      },
      error: e => {
        this.parametersBusy.set(false);
      }
    })
  }

  createResult(): any {
    return {
      name: this.resourceType()?.name,
      method: this.selectedCtor(),
    } as any;
  }
}

