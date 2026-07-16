/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable, signal } from "@angular/core";
import { WorkstationViewModel } from "../models/workstation-view-model";
import { OperatorViewModel } from "../models/operator-view-model";
import {
  OperatorManagementService,
  SkillManagementService,
} from "@api/services";
import { SnackbarService } from "@moryx/ngx-web-framework/services";
import { OperatorModel } from "@api/models/operator-model";
import { AssignableOperator } from "@api/models/assignable-operator";
import { OperatorSkill } from "../models/operator-skill-model";
import { SkillType } from "../models/skill-type-model";
import { SkillCreationContextModel } from "@api/models/skill-creation-context-model";
import {
  skillToOperatorSkill,
  skillTypeModelToModel,
} from "../models/model-converter";
import { SkillTypeModel } from "@api/models/skill-type-model";
import { SkillTypeCreationContextModel } from "@api/models/skill-type-creation-context-model";
import { IOperatorAssignable } from "@api/models/i-operator-assignable";

@Injectable({
  providedIn: "root",
})
export class AppStoreService {
  private operatorManagementService = inject(OperatorManagementService);
  private skillManagementService = inject(SkillManagementService);
  private snackbarService = inject(SnackbarService);

  private readonly _workstations = signal<WorkstationViewModel[]>([]);
  readonly workstations = this._workstations.asReadonly();
  private readonly _operators = signal<OperatorViewModel[]>([]);
  readonly operators = this._operators.asReadonly();
  private readonly _skills = signal<OperatorSkill[]>([]);
  readonly skills = this._skills.asReadonly();
  private readonly _skillTypes = signal<SkillType[]>([]);
  readonly skillTypes = this._skillTypes.asReadonly();
  private readonly _workstationSelected = signal<number>(0);
  readonly workstationSelected = this._workstationSelected.asReadonly();

  constructor() {
    this.initialize();
  }

  private initialize() {
    this.skillManagementService.getTypes().then((types) => {
      //types
      const typeModels = types.map(skillTypeModelToModel);
      this._skillTypes.set(typeModels);
    });

    //skill
    this.skillManagementService.getSkills().then((skills) => {
      const skillModels = skills.map(skillToOperatorSkill);
      this._skills.set(skillModels);
    });

    this.operatorManagementService.getResources_1().then((stations) => {
      const stationsModels = stations.map(
        (station) => new WorkstationViewModel(station)
      );
      this._workstations.set(stationsModels);
    });

    this.operatorManagementService
      .getAll()
      .then((operators) => this.mapOperatorsToModel(operators));
  }
  //#region Operator
  private mapOperatorsToModel(operators: AssignableOperator[]) {
    const operatorsModels = operators.map(
      (operator) => new OperatorViewModel(operator)
    );
    this._operators.set(operatorsModels);
  }

  public getSkillFromRemoteSource() {
    return this.skillManagementService.getSkills();
  }

  public getOperatorsByResourceId(resourceId: number) {
    return this.operatorManagementService.getOperatorsByResource({
      resourceId: resourceId,
    });
  }

  public assignOperator(
    workstation: WorkstationViewModel,
    operator: OperatorViewModel
  ) {
    //sign the operator in
    this.operatorManagementService
      .signIn({
        operatorIdentifier: operator.data.identifier ?? "",
        resourceId: workstation.data.id ?? 0,
      })
      .then(async () => {
        //update the current operator in the list of operators
        const operatorResult = await this.operatorManagementService
          .get({
            identifier: operator.data.identifier ?? "",
          });
        const assignedResource = this.workstations()
          .find((x) => x.data.id === workstation.data.id);
        if (!assignedResource) {
          return;
        }

        operator.data.assignedResources =
          operatorResult.assignedResources?.map(
            (x) => <IOperatorAssignable>{ id: x.id, name: x.name }
          );
        this._operators.update(current => [...current.filter(e => e.data.identifier != operatorResult.identifier), operator]);
      })
      .catch((error) => this.snackbarService.handleError(error));
  }

  public unassignOperator(
    operator: OperatorViewModel,
    workstation: WorkstationViewModel
  ) {
    this.operatorManagementService
      .signOut({
        operatorIdentifier: operator.data.identifier ?? "",
        resourceId: workstation.data.id ?? 0 })
      .then(async () => {
        const operatorResult = await this.operatorManagementService
          .get({
            identifier: operator.data.identifier ?? "",
          });

        operator.data.assignedResources =
          operatorResult.assignedResources?.map(
            (x) => <IOperatorAssignable>{ id: x.id, name: x.name }
          );
        this._operators.update(current => [...current.filter(e => e.data.identifier != operatorResult.identifier), operator]);
      });
  }

  public getWorkstationById(workstationId: number) {
    return this.workstations()
      .find((x) => x.data.id === workstationId);
  }

  private currentOperatorList() {
    return this.operators();
  }

  public deleteOperator(operator: OperatorViewModel) {
    const params = {
      operatorIdentifier: operator.data.identifier ?? "",
    };

    this.operatorManagementService.deleteOperator(params)
      .catch((error) => this.snackbarService.handleError(error));
  }

  public addOperator(operator: OperatorViewModel) {
    const data = <OperatorModel>{
      identifier: operator.data.identifier,
      pseudonym: operator.data.pseudonym,
      firstName: operator.data.firstName,
      lastName: operator.data.lastName,
    };
    this.operatorManagementService.add({
        body: data,
      })
      .then((identifier) => {
        const operators = [...this.currentOperatorList(), operator];
        this._operators.set(operators);
      })
      .catch((error) => this.snackbarService.handleError(error));
  }

  updateOperator(operator: AssignableOperator): Promise<void> {
    const model = this.currentOperatorList().find(
      (x) => x.data.identifier === operator.identifier
    );
    if (!model) {
      return new Promise(() => {});
    }

    return this.operatorManagementService.update({ identifier: model.data.identifier ?? "", body: operator })
      .then((result) => {
        return;
      })
      .catch((error) => this.snackbarService.handleError(error));
  }

  cancelEditing(operator: AssignableOperator) {
    const result = this.currentOperatorList().find(
      (x) => x.data.identifier === operator.identifier
    );
    return <OperatorModel>{
      identifier: result?.data.identifier,
      firstName: result?.data.firstName,
      lastName: result?.data.lastName,
      pseudonym: result?.data.pseudonym,
    };
  }

  getOperator(identifier: string): Promise<OperatorViewModel | undefined> {
    const result: OperatorViewModel | undefined = this.currentOperatorList().find(
      (x) => x.data.identifier === identifier
    );
    if (result) {
      return Promise.resolve(result);
    }

    return this.operatorManagementService.getAll().then(
      (operators) => {
        if (!operators) {
          return undefined;
        }
        this.mapOperatorsToModel(operators);
        return this.currentOperatorList().find(
          (x) => x.data.identifier === identifier
        );
      }
    );
  }
  //#endregion

  //#region  Skill

  getSkillType(id: number) {
    return this.skillManagementService.getType({ id });
  }

  addSkill(operator: OperatorViewModel, skill: OperatorSkill) {
    const data = <SkillCreationContextModel>{
      obtainedOn: skill.obtainedOn?.toISOString().split("T")[0],
      operatorIdentifier: operator.data.identifier ?? "",
      typeId: skill.typeId,
    };

    this.skillManagementService
      .create_1({
        body: data,
      })
      .then((skill) => {
        this._skills.update(current => [...current, skillToOperatorSkill(skill)]);
      })
      // TODO: snack back error
      .catch((e) => console.log(e));
  }

  deleteSkill(skill: OperatorSkill) {
    this.skillManagementService
      .deleteSkill({
        id: skill.id,
      })
      .then(() => {
        this._skills.update(current => current.filter(x => x.id != skill.id));
      })
      // TODO: snack back error
      .catch((e) => console.log(e));
  }

  deleteSkillType(skillType: SkillType) {
    this.skillManagementService
      .deleteType({
        id: skillType.id,
      })
      .then(() => {
        this._skillTypes.update(current => current.filter((e) => e.id != skillType.id));
      });
  }

  newSkillType(skillType: SkillType) {
    if (!skillType) {
      return;
    }

    const skillData = <SkillTypeCreationContextModel>{
      duration: skillType.duration,
      name: skillType.name,
      capabilities: skillType.acquiredCapabilities,
    };

    return this.skillManagementService.create({
        body: skillData,
      })
      .then((result) => {
        skillType.id = result.id ?? 0;
        this._skillTypes.update(current => [...current.filter(x => x.id != skillType.id), skillType]);
        return Promise.resolve(result);
      });
  }

  updateType(type: SkillType) {
    if (!type) {
      return;
    }

    const skillData = <SkillTypeModel>{
      id: type.id,
      name: type.name,
      duration: type.duration,
      capabilities: type.acquiredCapabilities,
    };
    this.skillManagementService
      .update_1({
        body: skillData,
      })
      .then(() => {
        this._skillTypes.update(current => [...current.filter(x => x.id != type.id), type]);
      });
  }

  getSkillTypePrototype() {
    return this.skillManagementService.getTypePrototype();
  }

  //#endregion
}

