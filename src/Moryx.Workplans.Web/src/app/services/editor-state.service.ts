import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { WorkplanSessionModel } from '../api/models/workplan-session-model';
import { WorkplanNodeModel } from '../api/models';

@Injectable({
  providedIn: 'root',
})
export class EditorStateService {
  private workplansChangedSource = new Subject<boolean>();
  private _currentWorkplan = new BehaviorSubject<WorkplanSessionModel | undefined>(undefined);
  private _selectedNode = new BehaviorSubject<number | undefined>(undefined);
  private isEditingProps = new BehaviorSubject<boolean>(false);
  private isEditingStep = new BehaviorSubject<WorkplanNodeModel | undefined>(undefined);
  private isCreatingStep = new BehaviorSubject<string | undefined>(undefined);

  workplanChangedSubject$ = this.workplansChangedSource.asObservable();
  currentWorkplan$ = this._currentWorkplan.asObservable();
  selectedNode$ = this._selectedNode.asObservable();
  isEditingProps$ = this.isEditingProps.asObservable();
  isEditingStep$ = this.isEditingStep.asObservable();
  isCreatingStep$ = this.isCreatingStep.asObservable();

  public get workplan() {
    return this._currentWorkplan.value;
  }

  workplanChanged() {
    this.workplansChangedSource.next(true);
  }

  setWorkplan(workplan: WorkplanSessionModel) {
    this._currentWorkplan.next(workplan);
    this.workplanChanged();
  }

  onNodeSelected(nodeId: number) {
    this._selectedNode.next(nodeId);
  }

  onNodeDeselected() {
    this._selectedNode.next(undefined);
  }

  startEditingProps() {
    this.stopEditingStep();
    this.stopCreatingStep();
    this.isEditingProps.next(true);
  }

  stopEditingProps() {
    if (this.isEditingProps.value) this.isEditingProps.next(false);
  }

  startEditingStep(nodeId: number) {
    this.stopEditingProps();
    this.stopEditingStep();
    this.stopCreatingStep();
    const node = this.workplan?.nodes?.find(node => node.id === nodeId);
    if (node) this.isEditingStep.next(node);
  }

  stopEditingStep() {
    if (this.isEditingStep.value) this.isEditingStep.next(undefined);
  }

  startCreatingStep(type: string) {
    this.stopEditingProps();
    this.stopEditingStep();
    this.stopCreatingStep();
    this.isCreatingStep.next(type);
  }

  stopCreatingStep() {
    if (this.isCreatingStep.value) this.isCreatingStep.next(undefined);
  }
}
