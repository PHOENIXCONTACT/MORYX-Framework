import { Injectable } from '@angular/core';
import { OperatorManagementService } from '../api/services';
import { AssignableOperator } from '../api/models/Moryx/Operators/assignable-operator';
import { OperatorModel } from '../api/models/Moryx/Operators/Endpoints/operator-model';

@Injectable({
  providedIn: 'root',
})
export class OperatorsService {
  private _available: boolean = true;
  public get available(): boolean {
    return this._available;
  }

  constructor(private operators: OperatorManagementService) {}

  async getOperators(): Promise<AssignableOperator[]> {
    if (!this._available) return [];

    let operators = [] as AssignableOperator[];
    await this.operators
      .getAll$Response()
      .toAsync()
      .then(o => (operators = o.body))
      .catch(e => (this._available = false));
    return operators;
  }

  async addOperator(identifier: string) {
    if (!this._available) return;

    return await this.operators.add({ body: { identifier: identifier } as OperatorModel }).toAsync();
  }
}
