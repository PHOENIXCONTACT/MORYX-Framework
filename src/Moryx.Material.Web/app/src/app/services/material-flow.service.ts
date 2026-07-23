import { Injectable } from '@angular/core';
import { MaterialContainer } from '../models/material-container';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MaterialFlowService {
  private _filter = new BehaviorSubject<string[]>([]);
  flatTypes: any[] | undefined;
  containers: MaterialContainer[] = [];
  $filter = this._filter.asObservable();

  constructor() {
    this.flatTypes = [
      {
        baseType: null,
        constructors: [],
        creatable: true,
        derivedTypes: ["ExtraBigMaterialContainer"],
        description: "Container that has a material in it.",
        displayName: "Material Container",
        name: "MaterialContainer",
      },
      {
        baseType: "MaterialContainer",
        constructors: [],
        creatable: true,
        derivedTypes: [],
        description: "Container that has big material in it.",
        displayName: "Extra Material Container",
        name: "ExtraBigMaterialContainer",
      }
    ];
    this.containers = [
      {
        icon: 'home',
        type: "SAC Cable",
        links: ['Packing Cell', 'Order: 1101010', 'SAC M3 FR'],
        instanceCount: 20,
        resource: 'F-Cables'
      },
      {
        icon: 'repartition',
        type: "O-Ring",
        links: ['Assembly Cell', 'Order: 1201010', 'O-Ring 2mm'],
        instanceCount: 50,
        resource: 'O-Rings'
      },
      {
        icon: 'key',
        type: "Flange",
        links: ['Assembly Cell', 'Order: 1301010', 'Male Flange ST'],
        instanceCount: 15,
        resource: 'M-Flanges'
      }
    ];
  }

  executeFilter(filter: string[] ) {
    this._filter.next(filter);
  }

}