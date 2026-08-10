/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, inject, signal, untracked, ChangeDetectionStrategy } from '@angular/core';
import { MatTable, MatTableModule } from '@angular/material/table';
import { TranslatePipe } from '@ngx-translate/core';
import { TranslationConstants } from '@app/extensions/translation-constants';
import { ReferenceTypeModel, ResourceModel, ResourceReferenceModel, ResourceReferenceRole } from '@api/models';
import { CacheResourceService } from '@app/services/cache-resource.service';
import { EditResourceService } from '@app/services/edit-resource.service';
import { Router } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { WordBreakPipe } from '@app/pipes/word-break.pipe';


@Component({
  selector: 'app-resource-references',
  templateUrl: './resource-references.html',
  styleUrls: ['./resource-references.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatExpansionModule,
    MatFormFieldModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatTableModule,
    TranslatePipe,
    MatButtonModule,
    WordBreakPipe,
]
})
export class ResourceReferences {
  private cacheResourceService = inject(CacheResourceService);
  private editResourceService = inject(EditResourceService);
  private router = inject(Router);

  protected readonly ResourceReferenceRole = ResourceReferenceRole;
  protected selectedTarget: ResourceModel | undefined;
  protected isEditMode = this.editResourceService.editing;
  protected referenceTypes = signal<ReferenceTypeModel[] | null | undefined>(undefined);
  protected selectedReferenceType = signal<ReferenceTypeModel | undefined>(undefined);
  protected selectedReference = signal<ResourceReferenceModel | undefined>(undefined);
  protected possibleResources = signal<ResourceModel[]>([]);

  protected TranslationConstants = TranslationConstants;

  protected compareWith = (o1: ResourceModel, o2: ResourceModel) => {
    return o1?.id === o2?.id;
  };

  constructor() {
    effect(() => {
      const resource = this.editResourceService.activeResource();
      if (!resource) {
        return;
      }
      untracked(() => {
        this.loadReferences(resource);
      });
    });
  }

  private loadReferences(resource: ResourceModel) {
    const resourceType = this.cacheResourceService.flatTypes?.find(t => t.name === resource?.type);

    // In the unlikely case that the resource types haven't been loaded yet,
    // navigate to the empty details screen.
    if (!resourceType) {
      this.router.navigate([`/details/${resource.id}`]);
      return;
    }
    this.referenceTypes.set(resourceType?.references);
    if (this.selectedReferenceType()) {
      this.onReferenceChanged(undefined);
    }
  }

  protected onReferenceChanged(type: ReferenceTypeModel | undefined) {
    this.selectedReferenceType.set(type);
    this.selectedReference.set(this.editResourceService.activeResource()?.references?.find(r => r.name == this.selectedReferenceType()?.name));

    this.possibleResources.set(this.getPossibleResources());
    if (!this.selectedReferenceType()?.isCollection && this.selectedReference()?.targets?.length) {
      this.selectedTarget = this.selectedReference()?.targets![0];
    } else {
      this.selectedTarget = undefined;
    }
  }

  protected addTarget(table: MatTable<ResourceModel>, target: ResourceModel) {
    if (!target || !this.selectedReference()) {
      return;
    }

    this.selectedReference()?.targets?.push(target);
    this.possibleResources.set(this.getPossibleResources());
    table.renderRows();
  }

  protected setTarget() {
    if (!this.selectedTarget || !this.selectedReference()) {
      this.resetTarget();
      return;
    }

    this.selectedReference.update(ref => {
      ref!.targets = [this.selectedTarget as ResourceModel];
      return ref;
    });
  }

  private resetTarget() {
    if (!this.selectedReference()) {
      return;
    }

    this.selectedReference.update(ref => {
      ref!.targets = []
      return ref;
    });
  }

  protected deleteTarget(target: ResourceModel) {
    if (!this.selectedReference()) {
      return;
    }

    this.selectedReference.update(ref => {
      ref!.targets = this.selectedReference()?.targets?.filter(t => t.id != target.id);
      return ref;
    });

    this.selectedTarget = undefined;
    this.possibleResources.set(this.getPossibleResources());
  }

  /**
   * Search through the list of available resources and list all resources
   * of the supported types and their subtypes.
   * @returns A list of resources that can be linked using the referenceType
   */
  private getPossibleResources(): ResourceModel[] {
    let possibleResources = [] as ResourceModel[];
    const supportedTypes = this.getAllSupportedTypes(this.selectedReferenceType()?.supportedTypes);
    this.cacheResourceService.flatResources()?.forEach(r => {
      if (supportedTypes.find(t => r.type === t) && this.editResourceService.activeResource()?.id != r.id) {
        possibleResources.push(r);
      }
    });

    if (this.selectedReferenceType()?.isCollection) {
      possibleResources = possibleResources.filter(r => !this.selectedReference()?.targets?.find(t => t.id == r.id));
    }
    possibleResources = possibleResources.sort((a, b) =>
      (a.name ?? '').localeCompare(b.name ?? '', undefined, { sensitivity: 'base' })
    );
    return possibleResources;
  }

  /**
   * Generate a list of the supported types including their subtypes as entries in the
   * list as well.
   * @param supportedRootTypes The root types to generate the combined list of root and subtypes from
   * @returns The combined list
   */
  private getAllSupportedTypes(supportedRootTypes: string[] | null | undefined): string[] {
    if (!supportedRootTypes) {
      return [];
    }

    const supportedSubTypes = Object.assign<string[], string[]>([], supportedRootTypes);
    for (let index = 0; index < supportedSubTypes.length; index++) {
      const rootType = this.cacheResourceService.flatTypes?.find(t => t.name == supportedSubTypes[index]);
      rootType?.derivedTypes?.forEach(t => {
        if (t.name && !supportedSubTypes.find(et => t.name === et)) {
          supportedSubTypes.push(t.name);
        }
      });
    }

    return supportedSubTypes;
  }

  protected getTargetNames(referenceType: ReferenceTypeModel): string {
    const reference = this.editResourceService.activeResource()?.references?.find(
      r => r.name === referenceType.name
    );
    const names = reference?.targets
      ?.map(target => target.name)
      .filter(Boolean)
      .join(', ');
    return names ? `: ${names}` : '';
  }

}
