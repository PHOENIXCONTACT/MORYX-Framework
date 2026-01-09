import { CdkDragDrop, CdkDragEnd, CdkDragStart, DragDropModule, DragRef } from '@angular/cdk/drag-drop';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, signal, viewChild, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuModule, MatMenuTrigger } from '@angular/material/menu';
import { MatDrawer, MatDrawerMode, MatSidenavModule } from '@angular/material/sidenav';
import { ActivatedRoute, ActivatedRouteSnapshot, ParamMap, Params, Router } from '@angular/router';
import { MoryxSnackbarService, PrototypeToEntryConverter } from '@moryx/ngx-web-framework';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { NodeConnectionPoint, WorkplanNodeClassification, WorkplanNodeModel, WorkplanStepRecipe } from '../../../api/models';
import { WorkplanEditingService } from '../../../api/services';
import { TranslationConstants } from '../../../extensions/translation-constants.extensions';
import { EditorStateService } from '../../../services/editor-state.service';
import { SessionsService } from '../../../services/sessions.service';
import { Position } from './position';
import { NodeConnectionPath, Segment } from './workplan-path';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { WorkplanPropertiesComponent } from './workplan-properties/workplan-properties.component';
import { NodePropertiesComponent } from './node-properties/node-properties.component';
import { StepCreatorComponent } from './step-creator/step-creator.component';
import { MatButtonModule } from '@angular/material/button';

enum EditQueries {
  Properties = 'properties',
  Node = 'node',
  New = 'new',
}

@Component({
    selector: 'app-editor',
    templateUrl: './editor.component.html',
    styleUrls: ['./editor.component.scss'],
    standalone: true,
    imports:[
      CommonModule,
      MatSidenavModule,
      MatIconModule,
      MatTabsModule,
      MatCardModule,
      MatDrawer,
      MatTooltipModule,
      DragDropModule,
      TranslateModule,
      MatProgressSpinnerModule,
      MatMenuModule,
      WorkplanPropertiesComponent,
      NodePropertiesComponent,
      StepCreatorComponent,
      MatButtonModule,
    ]
})
export class EditorComponent implements OnInit {
  pathMenuTrigger = viewChild.required<MatMenuTrigger>('pathMenuTrigger');
  stepMenuTrigger = viewChild.required<MatMenuTrigger>('stepMenuTrigger');
  availableSteps = signal<WorkplanStepRecipe[]>([]);
  inputIds = signal<string[]>([]);
  workplanPaths = signal<NodeConnectionPath[]>([]);
  isCreatingStep = signal(false);
  isEditingStep = signal(false);
  isEditingProps = signal(false);
  newStepPosition = signal<Position | undefined>(undefined);
  selectedNode = signal<number | undefined>(undefined);
  isLoading = signal(true);
  drawerIsOpen = signal(false);
  drawerMode = signal<MatDrawerMode>('side');
  dragPosition = signal<{ x: number; y: number }>({ x: 0, y: 0 });

  readonly size = 5000;
  readonly stepSize = 14;
  readonly nodeWidth = 8*this.stepSize;
  readonly nodeHeight = 4*this.stepSize;
  readonly editQuery = 'edit';
  readonly selectedQuery = 'selected';
  readonly typeQuery = 'type';
  readonly EditQueries = EditQueries;
  prevTouchPos = new Position(0, 0);

  private clickStartTime: number = 0;
  private sessionToken!: string;
  private stepMove: boolean = false;


  canvasPosition: Position = new Position(-this.size, -this.size);
  canvasScale: number = 1.0;
  cursorOffset = { x: 0, y: 0 };
  menuX: string = '0';
  menuY: string = '0';

  TranslationConstants = TranslationConstants;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private workplanEditing: WorkplanEditingService,
    private sessionService: SessionsService,
    private moryxSnackbar: MoryxSnackbarService,
    public dialog: MatDialog,
    public translate: TranslateService,
    public editorState: EditorStateService
  ) {
    // Ammending this strategy to use management, but this piece of code needs refactoring
    router.routeReuseStrategy.shouldReuseRoute = (future: ActivatedRouteSnapshot, curr: ActivatedRouteSnapshot) => {
      if (future.routeConfig?.path && future.routeConfig?.path !== 'session' && future.routeConfig?.path !== ':token')
        return false;

      if (curr.routeConfig?.path && curr.routeConfig?.path !== 'session' && future.routeConfig?.path !== ':token')
        return false;

      return future.paramMap.get('token') === curr.paramMap.get('token');
    };

    // React to changes in the state of the editor
    editorState.workplanChangedSubject$.subscribe(() => {this.scheduleRenderPaths(); this.gatherInputIds()});
    editorState.selectedNode$.subscribe(nodeId => (this.selectedNode.update(_=> nodeId)));
    editorState.isEditingProps$.subscribe(b => {
      this.drawerIsOpen.update(_=> b);
      this.isEditingProps.update(_=> b);
    });
    editorState.isEditingStep$.subscribe(stepId => {
      this.drawerIsOpen.update(_=> !!stepId);
      this.isEditingStep.update(_=> !!stepId);
    });
    editorState.isCreatingStep$.subscribe(type => {
      this.drawerIsOpen.update(_=> !!type);
      this.drawerMode.update(_=> !!type ? 'over' : 'side');
      this.isCreatingStep.update(_=> !!type);
    });

    // Configure initial state of the editor from route
    this.processInitialRoute();
  }

  private gatherInputIds() {
    const ids = this.editorState.workplan?.nodes!.flatMap(n => n.inputs!.map(i => "in_" + n.id + "-" + i.index));
    if(ids)
      this.inputIds.update(_=> ids);
    else
      this.inputIds.update(_=> []);
  }

  processInitialRoute() {
    const routeSnapshotParams = this.activatedRoute.snapshot.paramMap;
    this.sessionToken = String(routeSnapshotParams.get('token'));
    
    const routeSnapshotQueryParams = this.activatedRoute.snapshot.queryParamMap;
    this.checkPropertiesEditing(routeSnapshotQueryParams);
    this.checkStepEditing(routeSnapshotQueryParams);
    this.checkStepCreation(routeSnapshotQueryParams);
  }

  private checkPropertiesEditing(queries: ParamMap) {
    const editMode = queries.get(this.editQuery);
    if (editMode === EditQueries.Properties) this.editorState.startEditingProps();
  }

  private checkStepEditing(queries: ParamMap) {
    const stepId = Number(queries.get(this.selectedQuery));
    if (stepId) this.editorState.startEditingStep(stepId);
  }

  private checkStepCreation(queries: ParamMap) {
    const createdType = queries.get(this.typeQuery);
    if (queries.get(this.editQuery) === EditQueries.New && createdType) this.editorState.startCreatingStep(createdType);
  }

  ngOnInit(): void {
    this.workplanEditing.availableSteps().subscribe({
      next: steps => (this.availableSteps.update(_=> steps)),
      error: async (e: HttpErrorResponse) => await this.moryxSnackbar.handleError(e),
    });
    this.sessionService.getSession(this.sessionToken).subscribe({
      next: workplan => {
        // Todo: remove responsibility, sessionService should handle this
        this.editorState.setWorkplan(workplan);
      },
      error: async (e: HttpErrorResponse) => {
        await this.moryxSnackbar.handleError(e);
        this.sessionService.deactivateSession();
        this.router.navigate(['session', 'management']);
        this.isLoading.update(_=> false);
      },
      complete: () => {
        this.isLoading.update(_=> false);
      },
    });
  }

  onToggleDrawer(): void {
    if (this.isCreatingStep()) {
      this.editorState.stopCreatingStep();
    }

    if (this.selectedNode() && !this.drawerIsOpen()) {
      // ToDo: Check if query update on editorEvents can be done
      this.updateQuery({ edit: EditQueries.Node, type: null });
      this.editorState.startEditingStep(this.selectedNode()!);
    }
    else if (this.selectedNode() && this.drawerIsOpen()) {
      this.updateQuery({ edit: null });
      this.editorState.stopEditingStep();
    }
    else if (!this.selectedNode() && !this.drawerIsOpen()) {
      this.updateQuery({ edit: EditQueries.Properties, type: null });
      this.editorState.startEditingProps();
    }
    else if (!this.selectedNode() && this.drawerIsOpen()) {
      this.updateQuery({ edit: null, type: null });
      this.editorState.stopEditingProps();
    }
  }

  //#region Canvas
  onRegisterClickStart(): void {
    this.clickStartTime = Date.now();
  }

  onRegisterClickEnd(): void {
    const clickDuration: number = Date.now() - this.clickStartTime;
    if (clickDuration > 250) return;

    this.editorState.onNodeDeselected();
    this.editorState.stopEditingStep();
    this.editorState.stopEditingProps();
    
    this.updateQuery({ edit: null, selected: null, type: null }); 
  }

  positionOffset(position: number | undefined): number {
    const offsetPosition = this.size + (position ?? 0);
    const lockedPosition = Math.ceil(offsetPosition/this.stepSize) * this.stepSize;
    return lockedPosition;
  }

  scaleCanvas(event: WheelEvent) {
    event.preventDefault();
    const scale = this.canvasScale + event.deltaY * -0.001;
    this.canvasScale = Math.min(Math.max(0.125, scale), 1.2);
  }

  getCanvasStyling() {
    return {
      height: this.size * 2 + 'px',
      width: this.size * 2 + 'px',
      top: this.canvasPosition?.top + 'px',
      left: this.canvasPosition?.left + 'px',
      transform: 'scale(' + this.canvasScale + ')',
    };
  }
  //#endregion

  //#region Drag and Drop
  dragConstrainPoint = (point: any, dragRef: DragRef) => {
    // Consider scaling of parent element when dragging cards
    let zoomMoveXDifference = 0;
    let zoomMoveYDifference = 0;
    if (this.canvasScale != 1) {
      zoomMoveXDifference = (1 - this.canvasScale) * dragRef.getFreeDragPosition().x;
      zoomMoveYDifference = (1 - this.canvasScale) * dragRef.getFreeDragPosition().y;
    }

    return {
      x: (point.x - this.cursorOffset.x + zoomMoveXDifference) as number,
      y: (point.y - this.cursorOffset.y + zoomMoveYDifference) as number,
    };
  };

  startDragging(event: CdkDragStart) {
    // Consider scaling of parent element when dragging cards
    const position = {
      x: this.dragPosition().x * this.canvasScale,
      y: this.dragPosition().y * this.canvasScale,
    };
    let mouseEvent = event.event as MouseEvent;
    this.cursorOffset.x = mouseEvent.clientX - event.source.element.nativeElement.getBoundingClientRect().left;
    this.cursorOffset.y = mouseEvent.clientY - event.source.element.nativeElement.getBoundingClientRect().top;
    event.source._dragRef.setFreeDragPosition(position);
    (event.source._dragRef as any)._activeTransform = this.dragPosition;
    (event.source._dragRef as any)._applyRootElementTransform(this.dragPosition().x, this.dragPosition().y);
  }

  endDragging(event: CdkDragEnd, node: WorkplanNodeModel) {
    if (!this.editorState.workplan) return;

    // Copy changes respecting the scaling
    node.positionLeft = (node.positionLeft ?? 0) + Math.ceil(event.distance.x / this.canvasScale);
    node.positionTop = (node.positionTop ?? 0) + Math.ceil(event.distance.y / this.canvasScale);
    // Reset drag position as the position is now changed on the node
    this.dragPosition.update(_=> {
      return {x: 0, y: 0}
    });

    this.sessionService.updateSession(this.editorState.workplan).subscribe({
      next: session => this.editorState.setWorkplan(session),
      error: async (e: HttpErrorResponse) => await this.moryxSnackbar.handleError(e),
    });
  }

  allowDrop(event: DragEvent) {
    if (!this.stepMove && event.dataTransfer) {
      event.preventDefault();
      event.dataTransfer.dropEffect = 'move';
    }
  }

  recipeDropped(event: DragEvent) {
    const recipeString = event.dataTransfer?.getData('string');
    const recipe: WorkplanStepRecipe = recipeString ? JSON.parse(recipeString) : undefined;
    const stepRecipe = recipe.subworkplanId
      ? this.availableSteps().find(s => s.subworkplanId == recipe?.subworkplanId)
      : this.availableSteps().find(s => s.type == recipe?.type);

    if (!stepRecipe) return;

    this.newStepPosition.update(_=> new Position(event.offsetX - this.size, event.offsetY - this.size));
    stepRecipe.positionLeft = this.newStepPosition()?.left;
    stepRecipe.positionTop = this.newStepPosition()?.top;

    if (stepRecipe?.constructor && stepRecipe?.type) {
      this.updateQuery({ edit: EditQueries.New, selected: null, type: stepRecipe.type });
      this.editorState.onNodeDeselected();
      this.editorState.startCreatingStep(stepRecipe.type);
    } else {
      this.onStepCreated(stepRecipe);
    }
  }
  //#endregion

  onStepCreated(stepRecipe: WorkplanStepRecipe) {
    this.workplanEditing.addStep({ sessionId: this.sessionToken, body: stepRecipe }).subscribe({
      next: step => this.onStepCreationSuccessResponse(step),
      error: async (e: HttpErrorResponse) => await this.moryxSnackbar.handleError(e),
    });
  }

  private onStepCreationSuccessResponse(step: WorkplanNodeModel) {
    if (!this.editorState.workplan) return;

    this.editorState.workplan.nodes?.push(step);
    this.sessionService.registerUpdatedSession(this.editorState.workplan);
    this.editorState.stopCreatingStep();
    this.updateQuery({ edit: EditQueries.Node, selected: step.id, type: null });
    this.editorState.startEditingStep(step.id!);
    this.editorState.onNodeSelected(step.id!)
    this.editorState.workplanChanged();
  }

  onClickStep(node: WorkplanNodeModel) {
    if (!node.id) return;

    // ToDo: Check if query update on editorEvents can be done
    this.updateQuery({ edit: EditQueries.Node, selected: node.id });
    this.editorState.onNodeSelected(node.id);
    this.editorState.startEditingStep(node.id);
  }

  //--- Connect output to input
  connected(event: CdkDragDrop<WorkplanNodeModel[]>, node: WorkplanNodeModel, input: NodeConnectionPoint) {
    let sourceNode = <WorkplanNodeModel>event.item.data[0];
    var draggedConnector = <NodeConnectionPoint>event.item.data[1];
    this.workplanEditing
      .connectStep({
        sessionId: this.sessionToken,
        targetNodeId: node.id ?? 0,
        targetIndex: input.index ?? 0,
        body: { nodeId: sourceNode.id, index: draggedConnector.index },
      })
      .subscribe({
        next: session => {
          this.sessionService.registerUpdatedSession(session);
          this.editorState.setWorkplan(session);
        },
        error: async (e: HttpErrorResponse) => await this.moryxSnackbar.handleError(e),
      });
  }
  //---

  scheduleRenderPaths() {
    setTimeout(() => this.renderPaths(), 50);
  }

  private renderPaths(){
    const nodes = this.editorState.workplan?.nodes ?? [];
    const result = nodes.flatMap(node => { 
      return node.outputs!.flatMap(output => {
        return output.connections!.map(connection => {
          let path = NodeConnectionPath.findPath(node, output, connection, this.canvasScale, this.stepSize);
          path.endNode = nodes.find(n => n.id === connection.nodeId)!;
          path.endInput = path.endNode.inputs?.find(i => i.index === connection.index)!;
          return path;
        });
      })
    }) ?? [];
    this.workplanPaths.update(_=> result);
  }

  onPathDeleteClick() {
    if (!this.pathMenuTrigger().menuData) return;

    const data = this.pathMenuTrigger().menuData as NodeConnectionPath;
    this.workplanEditing
      .disconnectStep({
        sessionId: this.sessionToken,
        targetNodeId: data?.endNode.id ?? 0,
        targetIndex: data?.endInput.index ?? 0,
        body: { nodeId: data?.startNode.id, index: data?.startInput.index },
      })
      .subscribe({
        next: session => {
          this.sessionService.registerUpdatedSession(session);
          this.editorState.setWorkplan(session)
          this.workplanPaths.update(_=> this.workplanPaths().filter(p => p !== data));
          this.pathMenuTrigger().menuData = undefined;
        },
        error: async (err: HttpErrorResponse) => await this.moryxSnackbar.handleError(err),
      });
  }

  onStepDeleteClick() {
    if (!this.stepMenuTrigger().menuData) return;

    const data = this.stepMenuTrigger().menuData as WorkplanNodeModel;
    this.editorState.stopEditingStep();
    this.editorState.onNodeDeselected();
    this.updateQuery({ edit: null, selected: null });
    this.deleteStep(data);
  }

  deleteStep(data: WorkplanNodeModel) {
    this.workplanEditing
      .removeNode({
        sessionId: this.sessionToken,
        nodeId: data.id ?? 0,
      })
      .subscribe({
        next: session => {
          this.sessionService.registerUpdatedSession(session);
          this.editorState.setWorkplan(session);
          this.stepMenuTrigger().menuData = undefined;
        },
        error: async (err: HttpErrorResponse) => await this.moryxSnackbar.handleError(err),
      });
  }

  //#region Context Menu Functions
  openStepContextMenu(event: MouseEvent, node: WorkplanNodeModel) {
    event.preventDefault();
    this.menuX = event.clientX + 'px';
    this.menuY = event.clientY + 'px';
    this.openContextMenu(node);
    this.onClickStep(node);
  }

  openStepPressMenu(event: any, node: WorkplanNodeModel) {
    if (event.type != 'press' || event.pointer?.length > 1) return;
    event.preventDefault();
    this.menuX = event.pointers[0].clientX + 'px';
    this.menuY = event.pointers[0].clientY + 'px';
    this.openContextMenu(node);
  }

  private openContextMenu(node: WorkplanNodeModel) {
    this.stepMenuTrigger().menuData = node;
    this.stepMenuTrigger()?.openMenu();
  }

  openPathContextMenu(event: MouseEvent, path: NodeConnectionPath) {
    event.preventDefault();
    this.menuX = event.clientX + 'px';
    this.menuY = event.clientY + 'px';
    this.pathMenuTrigger().menuData = path;
    this.pathMenuTrigger()?.openMenu();
  }

  openPathTapMenu(event: any, path: NodeConnectionPath) {
    if (event.type != 'tap' || event.pointer?.length > 1) return;
    event.preventDefault();
    this.menuX = event.pointers[0].clientX + 'px';
    this.menuY = event.pointers[0].clientY + 'px';
    this.pathMenuTrigger().menuData = path;
    this.pathMenuTrigger()?.openMenu();
  }

  getPathMenuAreaStyles(segment: Segment): { [klass: string]: any } {
    return segment.width > segment.height
      ? { width: `${segment.width}px`, height: `${segment.height * 5}px`, top: `${-4}px` }
      : { width: `${segment.width * 5}px`, height: `${segment.height}px`, left: `${-4}px` };
  }

  isNodeUneditable(nodeId: number): boolean {
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
      queryParamsHandling: 'merge',
    });
  }
}
