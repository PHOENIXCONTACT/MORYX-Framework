/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { CdkDragDrop, CdkDragEnd, CdkDragStart, DragDropModule, DragRef, Point } from '@angular/cdk/drag-drop';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, effect, inject, OnInit, signal, untracked, viewChild, ChangeDetectionStrategy } from '@angular/core';
import { MatMenuModule, MatMenuTrigger } from '@angular/material/menu';
import { MatDrawer, MatDrawerMode, MatSidenavModule } from '@angular/material/sidenav';
import { ActivatedRoute, ActivatedRouteSnapshot, ParamMap, Params, Router } from '@angular/router';
import { SnackbarService } from '@moryx/ngx-web-framework/services';
import { TranslatePipe } from '@ngx-translate/core';
import {
  NodeConnectionPoint,
  WorkplanNodeClassification,
  WorkplanNodeModel,
  WorkplanStepRecipe
} from '@api/models';
import { WorkplanEditingService } from '@api/services';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { EditorStateService } from '@app/services/editor-state.service';
import { SessionsService } from '@app/services/sessions.service';
import { Position } from './position';
import { NodeConnectionPath, Segment } from './workplan-path';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { WorkplanProperties } from './workplan-properties/workplan-properties';
import { NodeProperties } from './node-properties/node-properties';
import { StepCreator } from './step-creator/step-creator';
import { MatButtonModule } from '@angular/material/button';

enum EditQueries {
  Properties = 'properties',
  Node = 'node',
  New = 'new',
}

@Component({
  selector: 'app-editor',
  templateUrl: './editor.html',
  styleUrls: ['./editor.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatSidenavModule,
    MatIconModule,
    MatTabsModule,
    MatCardModule,
    MatDrawer,
    MatTooltipModule,
    DragDropModule,
    TranslatePipe,
    MatProgressSpinnerModule,
    MatMenuModule,
    WorkplanProperties,
    NodeProperties,
    StepCreator,
    MatButtonModule
  ]
})
export class Editor implements OnInit {
  readonly pathMenuTrigger = viewChild.required<MatMenuTrigger>('pathMenuTrigger');
  readonly stepMenuTrigger = viewChild.required<MatMenuTrigger>('stepMenuTrigger');

  private activatedRoute = inject(ActivatedRoute);
  private router = inject(Router);
  private workplanEditingService = inject(WorkplanEditingService);
  private sessionService = inject(SessionsService);
  private snackbarService = inject(SnackbarService);
  protected editorState = inject(EditorStateService);

  protected availableSteps = signal<WorkplanStepRecipe[]>([]);
  protected inputIds = signal<string[]>([]);
  protected workplanPaths = signal<NodeConnectionPath[]>([]);
  protected newStepPosition = signal<Position | undefined>(undefined);
  protected selectedNode = this.editorState.selectedNode;
  protected isEditingProps = computed(() => this.editorState.isEditingProps());
  protected isEditingStep = computed(() => !!this.editorState.isEditingStep());
  protected isCreatingStep = computed(() => !!this.editorState.isCreatingStep());
  protected drawerIsOpen = computed(() => this.isEditingProps() || this.isEditingStep() || this.isCreatingStep());
  protected drawerMode = computed<MatDrawerMode>(() => this.isCreatingStep() ? 'over' : 'side');
  protected isLoading = signal(true);
  protected dragPosition = signal<{ x: number; y: number }>({x: 0, y: 0});

  protected readonly size = 5000;
  protected readonly stepSize = 14;
  protected readonly nodeWidth = 8 * this.stepSize;
  protected readonly nodeHeight = 4 * this.stepSize;
  protected readonly editQuery = 'edit';
  protected readonly selectedQuery = 'selected';
  protected readonly typeQuery = 'type';
  protected readonly EditQueries = EditQueries;
  protected prevTouchPos = new Position(0, 0);

  private clickStartTime: number = 0;
  private sessionToken!: string;
  private stepMove: boolean = false;

  protected canvasPosition: Position = new Position(-this.size, -this.size);
  protected canvasScale: number = 1.0;
  protected cursorOffset = {x: 0, y: 0};
  protected menuX: string = '0';
  protected menuY: string = '0';

  protected TranslationConstants = TranslationConstants;

  constructor() {
    // Amending this strategy to use management, but this piece of code needs refactoring
    this.router.routeReuseStrategy.shouldReuseRoute = (future: ActivatedRouteSnapshot, curr: ActivatedRouteSnapshot) => {
      if (future.routeConfig?.path && future.routeConfig?.path !== 'session' && future.routeConfig?.path !== ':token') {
        return false;
      }

      if (curr.routeConfig?.path && curr.routeConfig?.path !== 'session' && future.routeConfig?.path !== ':token') {
        return false;
      }

      return future.paramMap.get('token') === curr.paramMap.get('token');
    };

    // React to workplan changes
    effect(() => {
      this.editorState.workplanChanged();
      untracked(() => {
        this.scheduleRenderPaths();
        this.gatherInputIds();
      });
    });

    // Configure initial state of the editor from route
    this.processInitialRoute();
  }

  private gatherInputIds() {
    const ids = this.editorState.workplan?.nodes!.flatMap(n => n.inputs!.map(i => 'in_' + n.id + '-' + i.index));
    if (ids) {
      this.inputIds.set(ids);
    } else {
      this.inputIds.set([]);
    }
  }

  private processInitialRoute() {
    const routeSnapshotParams = this.activatedRoute.snapshot.paramMap;
    this.sessionToken = String(routeSnapshotParams.get('token'));

    const routeSnapshotQueryParams = this.activatedRoute.snapshot.queryParamMap;
    this.checkPropertiesEditing(routeSnapshotQueryParams);
    this.checkStepEditing(routeSnapshotQueryParams);
    this.checkStepCreation(routeSnapshotQueryParams);
  }

  private checkPropertiesEditing(queries: ParamMap) {
    const editMode = queries.get(this.editQuery);
    if (editMode === EditQueries.Properties) {
      this.editorState.startEditingProps();
    }
  }

  private checkStepEditing(queries: ParamMap) {
    const stepId = Number(queries.get(this.selectedQuery));
    if (stepId) {
      this.editorState.startEditingStep(stepId);
    }
  }

  private checkStepCreation(queries: ParamMap) {
    const createdType = queries.get(this.typeQuery);
    if (queries.get(this.editQuery) === EditQueries.New && createdType) {
      this.editorState.startCreatingStep(createdType);
    }
  }

  ngOnInit(): void {
    this.workplanEditingService.availableSteps()
      .then(steps => this.availableSteps.set(steps))
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
    this.sessionService.getSession(this.sessionToken)
      .then(workplan => {
        // Todo: remove responsibility, sessionService should handle this
        this.editorState.setWorkplan(workplan);
        this.isLoading.set(false);
      })
      .catch(async (e: HttpErrorResponse) => {
        await this.snackbarService.handleError(e);
        this.sessionService.deactivateSession();
        this.router.navigate(['session', 'management']);
        this.isLoading.set(false);
      });
  }

  protected onToggleDrawer(): void {
    if (this.isCreatingStep()) {
      this.editorState.stopCreatingStep();
    }

    if (this.selectedNode() && !this.drawerIsOpen()) {
      // ToDo: Check if query update on editorEvents can be done
      this.updateQuery({edit: EditQueries.Node, type: null});
      this.editorState.startEditingStep(this.selectedNode()!);
    } else if (this.selectedNode() && this.drawerIsOpen()) {
      this.updateQuery({edit: null});
      this.editorState.stopEditingStep();
    } else if (!this.selectedNode() && !this.drawerIsOpen()) {
      this.updateQuery({edit: EditQueries.Properties, type: null});
      this.editorState.startEditingProps();
    } else if (!this.selectedNode() && this.drawerIsOpen()) {
      this.updateQuery({edit: null, type: null});
      this.editorState.stopEditingProps();
    }
  }

  //#region Canvas
  protected onRegisterClickStart(): void {
    this.clickStartTime = Date.now();
  }

  protected onRegisterClickEnd(): void {
    const clickDuration: number = Date.now() - this.clickStartTime;
    if (clickDuration > 250) {
      return;
    }

    this.editorState.onNodeDeselected();
    this.editorState.stopEditingStep();
    this.editorState.stopEditingProps();

    this.updateQuery({edit: null, selected: null, type: null});
  }

  protected positionOffset(position: number | undefined): number {
    const offsetPosition = this.size + (position ?? 0);
    const lockedPosition = Math.ceil(offsetPosition / this.stepSize) * this.stepSize;
    return lockedPosition;
  }

  protected scaleCanvas(event: WheelEvent) {
    event.preventDefault();
    const scale = this.canvasScale + event.deltaY * -0.001;
    this.canvasScale = Math.min(Math.max(0.125, scale), 1.2);
  }

  //#endregion

  //#region Drag and Drop
  protected dragConstrainPoint = (point: Point, dragRef: DragRef) => {
    // Consider scaling of parent element when dragging cards
    let zoomMoveXDifference = 0;
    let zoomMoveYDifference = 0;
    if (this.canvasScale != 1) {
      zoomMoveXDifference = (1 - this.canvasScale) * dragRef.getFreeDragPosition().x;
      zoomMoveYDifference = (1 - this.canvasScale) * dragRef.getFreeDragPosition().y;
    }

    return {
      x: (point.x - this.cursorOffset.x + zoomMoveXDifference) as number,
      y: (point.y - this.cursorOffset.y + zoomMoveYDifference) as number
    };
  };

  protected startDragging(event: CdkDragStart) {
    // Consider scaling of parent element when dragging cards
    const position = {
      x: this.dragPosition().x * this.canvasScale,
      y: this.dragPosition().y * this.canvasScale
    };
    const mouseEvent = event.event as MouseEvent;
    this.cursorOffset.x = mouseEvent.clientX - event.source.element.nativeElement.getBoundingClientRect().left;
    this.cursorOffset.y = mouseEvent.clientY - event.source.element.nativeElement.getBoundingClientRect().top;
    event.source._dragRef.setFreeDragPosition(position);
    // eslint-disable-next-line @typescript-eslint/no-explicit-any -- accessing private CDK DragRef internals for zoom support
    (event.source._dragRef as any)._activeTransform = this.dragPosition;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    (event.source._dragRef as any)._applyRootElementTransform(this.dragPosition().x, this.dragPosition().y);
  }

  protected endDragging(event: CdkDragEnd, node: WorkplanNodeModel) {
    if (!this.editorState.workplan) {
      return;
    }

    // Copy changes respecting the scaling
    node.positionLeft = (node.positionLeft ?? 0) + Math.ceil(event.distance.x / this.canvasScale);
    node.positionTop = (node.positionTop ?? 0) + Math.ceil(event.distance.y / this.canvasScale);
    // Reset drag position as the position is now changed on the node
    this.dragPosition.set({x: 0, y: 0});

    this.sessionService.updateSession(this.editorState.workplan)
      .then(session => this.editorState.setWorkplan(session))
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
  }

  protected allowDrop(event: DragEvent) {
    if (!this.stepMove && event.dataTransfer) {
      event.preventDefault();
      event.dataTransfer.dropEffect = 'move';
    }
  }

  protected recipeDropped(event: DragEvent) {
    const recipeString = event.dataTransfer?.getData('string');
    const recipe: WorkplanStepRecipe = recipeString ? JSON.parse(recipeString) : undefined;
    const stepRecipe = recipe.subworkplanId
      ? this.availableSteps().find(s => s.subworkplanId == recipe?.subworkplanId)
      : this.availableSteps().find(s => s.type == recipe?.type);

    if (!stepRecipe) {
      return;
    }

    this.newStepPosition.set(new Position(event.offsetX - this.size, event.offsetY - this.size));
    stepRecipe.positionLeft = this.newStepPosition()?.left;
    stepRecipe.positionTop = this.newStepPosition()?.top;

    if (stepRecipe?.constructor && stepRecipe?.type) {
      this.updateQuery({edit: EditQueries.New, selected: null, type: stepRecipe.type});
      this.editorState.onNodeDeselected();
      this.editorState.startCreatingStep(stepRecipe.type);
    } else {
      this.onStepCreated(stepRecipe);
    }
  }

  //#endregion

  protected onStepCreated(stepRecipe: WorkplanStepRecipe) {
    this.workplanEditingService.addStep({sessionId: this.sessionToken, body: stepRecipe})
      .then(step => this.onStepCreationSuccessResponse(step))
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
  }

  private onStepCreationSuccessResponse(step: WorkplanNodeModel) {
    if (!this.editorState.workplan) {
      return;
    }

    this.editorState.workplan.nodes?.push(step);
    this.sessionService.registerUpdatedSession(this.editorState.workplan);
    this.editorState.stopCreatingStep();
    this.updateQuery({edit: EditQueries.Node, selected: step.id, type: null});
    this.editorState.startEditingStep(step.id!);
    this.editorState.onNodeSelected(step.id!);
    this.editorState.notifyWorkplanChanged();
  }

  protected onClickStep(node: WorkplanNodeModel) {
    if (!node.id) {
      return;
    }

    // ToDo: Check if query update on editorEvents can be done
    this.updateQuery({edit: EditQueries.Node, selected: node.id});
    this.editorState.onNodeSelected(node.id);
    this.editorState.startEditingStep(node.id);
  }

  //--- Connect output to input
  protected connected(event: CdkDragDrop<WorkplanNodeModel[]>, node: WorkplanNodeModel, input: NodeConnectionPoint) {
    const sourceNode = <WorkplanNodeModel>event.item.data[0];
    const draggedConnector = <NodeConnectionPoint>event.item.data[1];
    this.workplanEditingService
      .connectStep({
        sessionId: this.sessionToken,
        targetNodeId: node.id ?? 0,
        targetIndex: input.index ?? 0,
        body: {nodeId: sourceNode.id, index: draggedConnector.index}
      })
      .then(session => {
        this.sessionService.registerUpdatedSession(session);
        this.editorState.setWorkplan(session);
      })
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
  }

  //---

  private scheduleRenderPaths() {
    setTimeout(() => this.renderPaths(), 50);
  }

  private renderPaths() {
    const nodes = this.editorState.workplan?.nodes ?? [];
    const result = nodes.flatMap(node => {
      return node.outputs!.flatMap(output => {
        return output.connections!.map(connection => {
          const path = NodeConnectionPath.findPath(node, output, connection, this.canvasScale, this.stepSize);
          path.endNode = nodes.find(n => n.id === connection.nodeId)!;
          path.endInput = path.endNode.inputs!.find(i => i.index === connection.index)!;
          return path;
        });
      });
    }) ?? [];
    this.workplanPaths.set(result);
  }

  protected onPathDeleteClick() {
    if (!this.pathMenuTrigger().menuData) {
      return;
    }

    const data = this.pathMenuTrigger().menuData as NodeConnectionPath;
    this.workplanEditingService
      .disconnectStep({
        sessionId: this.sessionToken,
        targetNodeId: data?.endNode.id ?? 0,
        targetIndex: data?.endInput.index ?? 0,
        body: {nodeId: data?.startNode.id, index: data?.startInput.index}
      })
      .then(session => {
        this.sessionService.registerUpdatedSession(session);
        this.editorState.setWorkplan(session);
        this.workplanPaths.update(current => current.filter(p => p !== data));
        this.pathMenuTrigger().menuData = undefined;
      })
      .catch((err: HttpErrorResponse) => this.snackbarService.handleError(err));
  }

  protected onStepDeleteClick() {
    if (!this.stepMenuTrigger().menuData) {
      return;
    }

    const data = this.stepMenuTrigger().menuData as WorkplanNodeModel;
    this.editorState.stopEditingStep();
    this.editorState.onNodeDeselected();
    this.updateQuery({edit: null, selected: null});
    this.deleteStep(data);
  }

  private deleteStep(data: WorkplanNodeModel) {
    this.workplanEditingService
      .removeNode({
        sessionId: this.sessionToken,
        nodeId: data.id ?? 0
      })
      .then(session => {
        this.sessionService.registerUpdatedSession(session);
        this.editorState.setWorkplan(session);
        this.stepMenuTrigger().menuData = undefined;
      })
      .catch((err: HttpErrorResponse) => this.snackbarService.handleError(err));
  }

  //#region Context Menu Functions
  protected openStepContextMenu(event: MouseEvent, node: WorkplanNodeModel) {
    event.preventDefault();
    this.menuX = event.clientX + 'px';
    this.menuY = event.clientY + 'px';
    this.openContextMenu(node);
    this.onClickStep(node);
  }

  protected openStepPressMenu(event: Event, node: WorkplanNodeModel) {
    const hammerEvent = event as unknown as HammerInput;
    if (hammerEvent.type != 'press' || hammerEvent.pointers.length > 1) {
      return;
    }
    hammerEvent.preventDefault();
    this.menuX = hammerEvent.pointers[0].clientX + 'px';
    this.menuY = hammerEvent.pointers[0].clientY + 'px';
    this.openContextMenu(node);
  }

  private openContextMenu(node: WorkplanNodeModel) {
    this.stepMenuTrigger().menuData = node;
    this.stepMenuTrigger()?.openMenu();
  }

  protected openPathContextMenu(event: MouseEvent, path: NodeConnectionPath) {
    event.preventDefault();
    this.menuX = event.clientX + 'px';
    this.menuY = event.clientY + 'px';
    this.pathMenuTrigger().menuData = path;
    this.pathMenuTrigger()?.openMenu();
  }

  protected openPathTapMenu(event: Event, path: NodeConnectionPath) {
    const hammerEvent = event as unknown as HammerInput;
    if (hammerEvent.type != 'tap' || hammerEvent.pointers.length > 1) {
      return;
    }
    hammerEvent.preventDefault();
    this.menuX = hammerEvent.pointers[0].clientX + 'px';
    this.menuY = hammerEvent.pointers[0].clientY + 'px';
    this.pathMenuTrigger().menuData = path;
    this.pathMenuTrigger()?.openMenu();
  }

  protected getPathMenuAreaStyles(segment: Segment): Record<string, string> {
    return segment.width > segment.height
      ? {width: `${segment.width}px`, height: `${segment.height * 5}px`, top: `${-4}px`}
      : {width: `${segment.width * 5}px`, height: `${segment.height}px`, left: `${-4}px`};
  }

  protected isNodeUneditable(nodeId: number): boolean {
    const node = this.editorState.workplan?.nodes?.find(n => n.id === nodeId);
    return (
      node?.classification === WorkplanNodeClassification.Input ||
      node?.classification === WorkplanNodeClassification.Output
    );
  }

  //#endregion

  private updateQuery(queryParams: Params): void {
    this.router.navigate([], {
      queryParams: queryParams,
      queryParamsHandling: 'merge'
    });
  }
}

