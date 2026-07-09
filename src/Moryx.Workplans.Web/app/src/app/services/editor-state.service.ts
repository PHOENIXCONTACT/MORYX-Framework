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
  workplanChanged = signal(0);
  currentWorkplan = signal<WorkplanSessionModel | undefined>(undefined);
  selectedNode = signal<number | undefined>(undefined);
  isEditingProps = signal(false);
  isEditingStep = signal<WorkplanNodeModel | undefined>(undefined);
  isCreatingStep = signal<string | undefined>(undefined);

  public get workplan() {
    return this.currentWorkplan();
  }

  notifyWorkplanChanged() {
    this.workplanChanged.update(v => v + 1);
  }

  setWorkplan(workplan: WorkplanSessionModel) {
    this.currentWorkplan.set(workplan);
    this.notifyWorkplanChanged();
  }

  onNodeSelected(nodeId: number) {
    this.selectedNode.set(nodeId);
  }

  onNodeDeselected() {
    this.selectedNode.set(undefined);
  }

  startEditingProps() {
    this.stopEditingStep();
    this.stopCreatingStep();
    this.isEditingProps.set(true);
  }

  stopEditingProps() {
    if (this.isEditingProps()) {
      this.isEditingProps.set(false);
    }
  }

  startEditingStep(nodeId: number) {
    this.stopEditingProps();
    this.stopEditingStep();
    this.stopCreatingStep();
    const node = this.workplan?.nodes?.find(node => node.id === nodeId);
    if (node) {
      this.isEditingStep.set(node);
    }
  }

  stopEditingStep() {
    if (this.isEditingStep()) {
      this.isEditingStep.set(undefined);
    }
  }

  startCreatingStep(type: string) {
    this.stopEditingProps();
    this.stopEditingStep();
    this.stopCreatingStep();
    this.isCreatingStep.set(type);
  }

  stopCreatingStep() {
    if (this.isCreatingStep()) {
      this.isCreatingStep.set(undefined);
    }
  }
}
