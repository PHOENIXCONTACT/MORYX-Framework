/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Injectable, signal } from '@angular/core';
import { WorkplanSessionModel } from '@api/models/workplan-session-model';
import { WorkplanNodeModel } from '@api/models';

@Injectable({
  providedIn: 'root',
})
export class EditorStateService {
  private readonly _workplanChanged = signal(0);
  readonly workplanChanged = this._workplanChanged.asReadonly();
  private readonly _currentWorkplan = signal<WorkplanSessionModel | undefined>(undefined);
  readonly currentWorkplan = this._currentWorkplan.asReadonly();
  private readonly _selectedNode = signal<number | undefined>(undefined);
  readonly selectedNode = this._selectedNode.asReadonly();
  private readonly _isEditingProps = signal(false);
  readonly isEditingProps = this._isEditingProps.asReadonly();
  private readonly _isEditingStep = signal<WorkplanNodeModel | undefined>(undefined);
  readonly isEditingStep = this._isEditingStep.asReadonly();
  private readonly _isCreatingStep = signal<string | undefined>(undefined);
  readonly isCreatingStep = this._isCreatingStep.asReadonly();

  public get workplan() {
    return this.currentWorkplan();
  }

  notifyWorkplanChanged() {
    this._workplanChanged.update(v => v + 1);
  }

  setWorkplan(workplan: WorkplanSessionModel) {
    this._currentWorkplan.set(workplan);
    this.notifyWorkplanChanged();
  }

  onNodeSelected(nodeId: number) {
    this._selectedNode.set(nodeId);
  }

  onNodeDeselected() {
    this._selectedNode.set(undefined);
  }

  startEditingProps() {
    this.stopEditingStep();
    this.stopCreatingStep();
    this._isEditingProps.set(true);
  }

  stopEditingProps() {
    if (this.isEditingProps()) {
      this._isEditingProps.set(false);
    }
  }

  startEditingStep(nodeId: number) {
    this.stopEditingProps();
    this.stopEditingStep();
    this.stopCreatingStep();
    const node = this.workplan?.nodes?.find(node => node.id === nodeId);
    if (node) {
      this._isEditingStep.set(node);
    }
  }

  stopEditingStep() {
    if (this.isEditingStep()) {
      this._isEditingStep.set(undefined);
    }
  }

  startCreatingStep(type: string) {
    this.stopEditingProps();
    this.stopEditingStep();
    this.stopCreatingStep();
    this._isCreatingStep.set(type);
  }

  stopCreatingStep() {
    if (this.isCreatingStep()) {
      this._isCreatingStep.set(undefined);
    }
  }
}
