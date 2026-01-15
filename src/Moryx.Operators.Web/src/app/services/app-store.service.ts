/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable } from "@angular/core";
import { BehaviorSubject, firstValueFrom, Observable } from "rxjs";
import { WorkstationViewModel } from "../models/workstation-view-model";
import { OperatorViewModel } from "../models/operator-view-model";
import {
  OperatorManagementService,
  SkillManagementService,
} from "../api/services";
import { MoryxSnackbarService } from "@moryx/ngx-web-framework";
import { OperatorModel } from "../api/models/operator-model";
import { AssignableOperator } from "../api/models/assignable-operator";
import { OperatorSkill } from "../models/operator-skill-model";
import { SkillType } from "../models/skill-type-model";
import { SKILLS, SKILL_TYPES } from "../models/dummy-data";
import { SkillCreationContextModel } from "../api/models/skill-creation-context-model";
import {
  skillToOperatorSkill,
  skillTypeModelToModel,
} from "../models/model-converter";
import { SkillTypeModel } from "../api/models/skill-type-model";
import { SkillTypeCreationContextModel } from "../api/models/skill-type-creation-context-model";
import { AttendableResourceModel } from "../api/models/attendable-resource-model";
import { SkillModel } from "../api/models/skill-model";
import { IOperatorAssignable } from "../api/models/i-operator-assignable";

@Injectable({
  providedIn: "root",
})
export class AppStoreService {
  private _workstations = new BehaviorSubject<WorkstationViewModel[]>([]);
  private _operators = new BehaviorSubject<OperatorViewModel[]>([]);
  private _skills = new BehaviorSubject<OperatorSkill[]>([]);
  private _skillTypes = new BehaviorSubject<SkillType[]>([]);
  private _workstationSelected = new BehaviorSubject<number>(0);

  public workstations$ = this._workstations.asObservable();
  public operators$ = this._operators.asObservable();
  public skills$ = this._skills.asObservable();
  public skillTypes$ = this._skillTypes.asObservable();
  public workstationSelected$ = this._workstationSelected.asObservable();

  constructor(
    private operatorManagementService: OperatorManagementService,
    private skillManagementService: SkillManagementService,
    private moryxSnackbar: MoryxSnackbarService
  ) {
    this.initialize();
  }

  private initialize() {
    this.skillManagementService.getTypes().subscribe((types) => {
      //types
      const typeModels = types.map(skillTypeModelToModel);
      this._skillTypes.next(typeModels);
    });

    //skill
    this.skillManagementService.getSkills().subscribe((skills) => {
      const skillModels = skills.map(skillToOperatorSkill);
      this._skills.next(skillModels);
    });

    this.operatorManagementService.getResources_1().subscribe((stations) => {
      const stationsModels = stations.map(
        (station) => new WorkstationViewModel(station)
      );
      this._workstations.next(stationsModels);
    });

    this.operatorManagementService
      .getAll()
      .subscribe((operators) => this.mapOperatorsToModel(operators));
  }
  //#region Operator
  private mapOperatorsToModel(operators: AssignableOperator[]) {
    this.skills$.subscribe((skills) => {
      const operatorsModels = operators.map(
        (operator) => new OperatorViewModel(operator)
      );

      this._operators.next(operatorsModels);
    });
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
    return this.operatorManagementService
      .signIn({
        operatorIdentifier: operator.data.identifier ?? "",
        resourceId: workstation.data.id ?? 0,
      })
      .subscribe({
        next: (result) => {
          //update the current operator in the list of operators
          this.operatorManagementService
            .get({
              identifier: operator.data.identifier ?? "",
            })
            .subscribe((operatorResult) => {
              const assignedResource = this._workstations
                .getValue()
                .find((x) => x.data.id === workstation.data.id);
              if (!assignedResource) return;

              operator.data.assignedResources =
                operatorResult.assignedResources?.map(
                  (x) => <IOperatorAssignable>{ id: x.id, name: x.name }
                );
              this._operators.next([...this._operators.getValue().filter(e => e.data.identifier!= operatorResult.identifier),operator]);
            });
        },
        error: (error) => this.moryxSnackbar.handleError(error),
      });
  }

  public unassignOperator(
    operator: OperatorViewModel,
    workstation: WorkstationViewModel
  ) {
    this.operatorManagementService
      .signOut({ 
        operatorIdentifier: operator.data.identifier ?? "", 
        resourceId: workstation.data.id ?? 0 })
      .subscribe((operatorResult) => {

          this.operatorManagementService
          .get({
            identifier: operator.data.identifier ?? "",
          })
          .subscribe((operatorResult) => {
            
            operator.data.assignedResources =
              operatorResult.assignedResources?.map(
                (x) => <IOperatorAssignable>{ id: x.id, name: x.name }
              );
            this._operators.next([...this._operators.getValue().filter(e => e.data.identifier!= operatorResult.identifier),operator]);
          });
      });
  }

  public getWorkstationById(workstationId: number) {
    return this._workstations
      .getValue()
      .find((x) => x.data.id === workstationId);
  }

  private currentOperatorList() {
    return this._operators.getValue();
  }

  public deleteOperator(operator: OperatorViewModel) {
    const params = {
      operatorIdentifier: operator.data.identifier ?? "",
    };

    this.operatorManagementService
      .deleteOperator(params)
      .toAsync()
      .catch((error) => this.moryxSnackbar.handleError(error));
  }

  public addOperator(operator: OperatorViewModel) {
    const data = <OperatorModel>{
      identifier: operator.data.identifier,
      pseudonym: operator.data.pseudonym,
      firstName: operator.data.firstName,
      lastName: operator.data.lastName,
    };
    this.operatorManagementService
      .add({
        body: data,
      })
      .toAsync()
      .then((identifier) => {
        const operators = [...this.currentOperatorList(), operator];
        this._operators.next(operators);
      })
      .catch((error) => this.moryxSnackbar.handleError(error));
  }

  updateOperator(operator: AssignableOperator): Promise<void> {
    const model = this.currentOperatorList().find(
      (x) => x.data.identifier === operator.identifier
    );
    if (!model) return new Promise(() => {});

    return this.operatorManagementService
      .update({ identifier: model.data.identifier ?? "", body: operator })
      .toAsync()
      .then((result) => {
        return;
      })
      .catch((error) => this.moryxSnackbar.handleError(error));
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
    let result: OperatorViewModel | undefined = this.currentOperatorList().find(
      (x) => x.data.identifier === identifier
    );
    if (result) return Promise.resolve(result);

    return firstValueFrom(this.operatorManagementService.getAll()).then(
      (operators) => {
        if (!operators) return undefined;
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
      .subscribe({
        next: (skill) => {
          this._skills.next([
            ...this._skills.getValue(),
            skillToOperatorSkill(skill),
          ]);
        },
        // TODO: snack back error
        error: (e) => console.log(e),
      });
  }

  deleteSkill(skill: OperatorSkill) {
    this.skillManagementService
      .deleteSkill({
        id: skill.id,
      })
      .subscribe({
        next: (result) => {
          this._skills.next([
            ...this._skills.getValue().filter(x => x.id != skill.id)]);
        },
        // TODO: snack back error
        error: (e) => console.log(e),
      });
  }

  deleteSkillType(skillType: SkillType) {
    this.skillManagementService
      .deleteType({
        id: skillType.id,
      })
      .subscribe((result) => {
        this._skillTypes.next(
          this._skillTypes.getValue().filter((e) => e.id != skillType.id)
        );
      });
  }

  newSkillType(skillType: SkillType) {
    if (!skillType) return;

    const skillData = <SkillTypeCreationContextModel>{
      duration: skillType.duration,
      name: skillType.name,
      capabilities: skillType.acquiredCapabilities,
    };

    const addAsync = firstValueFrom(
      this.skillManagementService.create({
        body: skillData,
      })
    );

    return addAsync.then((result) => {
      skillType.id = result.id ?? 0;
      this._skillTypes.next([
        ...this._skillTypes.getValue().filter((x) => x.id != skillType.id),
        skillType,
      ]);
      return Promise.resolve(result);
    });
  }

  updateType(type: SkillType) {
    if (!type) return;

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
      .subscribe((result) => {
        this._skillTypes.next([
          ...this._skillTypes.getValue().filter((x) => x.id != type.id),
          type,
        ]);
      });
  }

  getSkillTypePrototype() {
    return firstValueFrom(this.skillManagementService.getTypePrototype());
  }

  //#endregion
}

