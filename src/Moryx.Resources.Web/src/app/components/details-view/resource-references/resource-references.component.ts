import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { MatTable, MatTableModule } from '@angular/material/table';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ReferenceTypeModel, ResourceModel, ResourceReferenceModel } from '../../../api/models';
import { CacheResourceService } from '../../../services/cache-resource.service';
import { EditResourceService } from '../../../services/edit-resource.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-resource-references',
  templateUrl: './resource-references.component.html',
  styleUrls: ['./resource-references.component.scss'],
  imports: [
    CommonModule,
    MatExpansionModule,
    MatFormFieldModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatTableModule,
    TranslateModule,
    MatButtonModule,
  ],
  standalone: true,
})
export class ResourceReferencesComponent implements OnInit, OnDestroy {
  resource: ResourceModel | undefined;
  references: ResourceReferenceModel[] | null | undefined;
  selectedTarget: ResourceModel | undefined;

  referenceTypes = signal<ReferenceTypeModel[] | null | undefined>(undefined);
  selectedReferenceType = signal<ReferenceTypeModel | undefined>(undefined);
  selectedReference = signal<ResourceReferenceModel | undefined>(undefined);

  possibleResources = signal<ResourceModel[]>([]);

  private serviceSubscription?: Subscription;
  TranslationConstants = TranslationConstants;

  compareWith = (o1: any, o2: any) => {
    return o1?.id === o2?.id;
  };

  constructor(
    private cacheService: CacheResourceService,
    public editService: EditResourceService,
    public translate: TranslateService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.serviceSubscription = this.editService.activeResource$.subscribe(resource => {
      if (!resource) return;
      this.loadReferences(resource);
    });
  }

  loadReferences(resource: ResourceModel) {
    var resourceType = this.cacheService.flatTypes?.find(t => t.name === resource?.type);

    // In the unlikely case that the resource types haven't been loaded yet,
    // navigate to the empty details screen.
    if (!resourceType) {
      this.router.navigate([`/details/${resource.id}`]);
      return;
    }

    this.resource = resource;
    this.references = resource.references;
      this.referenceTypes.update(() => resourceType?.references);
    if (this.selectedReferenceType()) {
      this.onReferenceChanged(undefined);
    }
  }

  ngOnDestroy(): void {
    this.serviceSubscription?.unsubscribe();
  }

  onReferenceChanged(type: ReferenceTypeModel | undefined) {
    this.selectedReferenceType.update(() => type);
    this.selectedReference.update(() => this.references?.find(r => r.name == this.selectedReferenceType()?.name));

    this.possibleResources.update(() => this.getPossibleResources());
    if (!this.selectedReferenceType()?.isCollection && this.selectedReference()?.targets?.length) {
      this.selectedTarget = this.selectedReference()?.targets![0];
    } else {
      this.selectedTarget = undefined;
    }
  }

  addTarget(table: MatTable<ResourceModel>) {
    if (!this.selectedTarget || !this.selectedReference()) return;

    this.selectedReference()?.targets?.push(this.selectedTarget as ResourceModel);
    this.possibleResources.update(() => this.getPossibleResources());
    this.selectedTarget = undefined;
    table.renderRows();
  }

  setTarget() {
    if (!this.selectedTarget || !this.selectedReference()) {
      this.resetTarget();
      return;
    }

    if (this.selectedReference()?.targets?.length) {
      this.selectedReference.update(ref => ({
        ...ref,
        targets: [this.selectedTarget as ResourceModel],
      }));
    } else {
      this.selectedReference()?.targets?.push(this.selectedTarget as ResourceModel);
    }
  }

  private resetTarget() {
    if (!this.selectedReference()) return;

    this.selectedReference.update(ref => {
      ref!.targets = []
      return ref;
    });
  }

  deleteTarget(target: ResourceModel) {
    if (!this.selectedReference) return;

    this.selectedReference.update(ref => ({
      ...ref,
      targets: this.selectedReference()?.targets?.filter(t => t.id != target.id),
    }));

    this.possibleResources.update(() => this.getPossibleResources());
  }

  getReferenceForType(type: ReferenceTypeModel): ResourceReferenceModel {
    return this.references?.find(r => r.name == type.name) ?? {};
  }

  /**
   * Search through the list of available resources and list all resources
   * of the supported types and their subtypes.
   * @param referenceType The reference type the Resources should be compatible with
   * @returns A list of resources that can be linked using the referenceType
   */
  getPossibleResources(): ResourceModel[] {
    var possibleResources = [] as ResourceModel[];
    var supportedTypes = this.getAllSupportedTypes(this.selectedReferenceType()?.supportedTypes);
    this.cacheService.flatResources.getValue()?.forEach(r => {
      if (supportedTypes.find(t => r.type === t) && this.resource?.id != r.id) possibleResources.push(r);
    });

    if (this.selectedReferenceType()?.isCollection)
      possibleResources = possibleResources.filter(r => !this.selectedReference()?.targets?.find(t => t.id == r.id));

    return possibleResources;
  }

  /**
   * Generate a list of the supported types including their subtypes as entries in the
   * list as well.
   * @param supportedRootTypes The root types to generate the combined list of root and subtypes from
   * @returns The combined list
   */
  private getAllSupportedTypes(supportedRootTypes: string[] | null | undefined): string[] {
    if (!supportedRootTypes) return [];

    var supportedSubTypes = Object.assign<string[], string[]>([], supportedRootTypes);
    for (let index = 0; index < supportedSubTypes.length; index++) {
      var rootType = this.cacheService.flatTypes?.find(t => t.name == supportedSubTypes[index]);
      rootType?.derivedTypes?.forEach(t => {
        if (t.name && !supportedSubTypes.find(et => t.name === et)) supportedSubTypes.push(t.name);
      });
    }

    return supportedSubTypes;
  }
}
