/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, effect, inject, OnInit, signal, viewChild, ChangeDetectionStrategy, DestroyRef } from "@angular/core";

import { MatTree, MatTreeModule } from "@angular/material/tree";
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { ProcessHolderStreamService } from "@app/services/process-holder-stream.service";
import { MatTooltipModule } from "@angular/material/tooltip";
import { HttpErrorResponse } from "@angular/common/http";
import { SnackbarService } from "@moryx/ngx-web-framework/services";
import { EmptyState } from "@moryx/ngx-web-framework/empty-state";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { TranslationConstants } from "@app/extensions/translation-constants.extensions";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { firstValueFrom } from "rxjs";
import { ProcessEngineService } from "@api/services";
import { ProcessHolderGroup } from "@app/models/process-holder-group-model";
import ProcessHolderNode from "../../models/process-holder-node";
import { Category } from "@api/models/category";
import { ConvertToNode, ConvertToProcessHolderGroup } from "@app/models/converter";
import { ProcessHolderGroupModelArrayApiResponse } from "@api/models/process-holder-group-model-array-api-response";

@Component({
  selector: "app-process-holders",
  imports: [
    MatTreeModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    EmptyState,
    MatFormFieldModule,
    ReactiveFormsModule,
    FormsModule,
    MatIconModule,
    MatInputModule,
    TranslatePipe
  ],
  templateUrl: "./process-holders.html",
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: "./process-holders.scss",
  host: {
    '(window:beforeunload)': 'disconnectEvents()'
  }
})
export class ProcessHolders implements OnInit {
  private destroyRef = inject(DestroyRef);
  protected processHolderGroups = signal<Array<ProcessHolderGroup>>([]);
  protected dataSource = signal<Array<ProcessHolderNode>>([]);
  protected loading = signal(false);
  protected filterText = signal("");
  protected visualizationCategory = Category;

  protected TranslationConstants = TranslationConstants;

  protected hasChild = (_: number, node: ProcessHolderNode) =>
    !!node.children && node.children.length > 0;
  protected childrenAccessor = (node: ProcessHolderNode) => node.children ?? [];

  private _processHolderStreamService = inject(ProcessHolderStreamService);
  private _processService = inject(ProcessEngineService);
  private _snackbarService = inject(SnackbarService);
  private _translate = inject(TranslateService);

  private readonly _tree = viewChild<MatTree<ProcessHolderNode, ProcessHolderNode>>("tree");

  constructor() {
    this.destroyRef.onDestroy(() => this.disconnectEvents());

    effect(() => {
      const group = this._processHolderStreamService.updatedProcessHolderGroups();
      if (group) {
        this.updateTree(ConvertToProcessHolderGroup(group));
      }
    });
  }

  ngOnInit(): void {
    this.loading.set(true);
    this._processHolderStreamService.connect();
    this._processService.getGroups().then((response: ProcessHolderGroupModelArrayApiResponse) => {
      console.log(response);
      this.processHolderGroups.set(response.data?.map((x) => ConvertToProcessHolderGroup(x)) ?? []);
      this.buildTree(this.processHolderGroups());
      this.loading.set(false);
    }).catch((e: HttpErrorResponse) => {
      this.loading.set(false);
      this.processHolderGroups.set([]);
      this.buildTree([]);
      this._snackbarService.handleError(e);
    });
  }

  private async getTranslations(): Promise<{ [key: string]: string }> {
    return await firstValueFrom(this._translate
      .get([TranslationConstants.PROCESS_HOLDER_GROUPS.OPERATION_SUCCEEDED]));
  }


  private updateTree(group: ProcessHolderGroup) {
    const node = ConvertToNode(group);
    this.dataSource.update((nodes) => {
      const foundNode = nodes.find((x) => x.data.id == group.id);
      if (foundNode) {
        Object.assign(foundNode, node);
        foundNode.children = node.children ? [...node.children] : undefined;
        if (this._tree()?.isExpanded(foundNode)) {
          this._tree()?.collapse(foundNode);
          this._tree()?.expand(foundNode);
        }
      } else {
        nodes.push(node);
      }
      return nodes;
    });
  }

  private buildTree(groups: ProcessHolderGroup[]) {
    const nodes: ProcessHolderNode[] = [];
    for (const group of groups) {
      const node = ConvertToNode(group);
      nodes.push(node);
    }
    this.dataSource.set(nodes);
  }

  protected resetGroup(id: number) {
    this._processService
      .resetGroup({
        id,
      })
      .then(() => this.getTranslations().then(values => this._snackbarService.showSuccess(values[TranslationConstants.PROCESS_HOLDER_GROUPS.OPERATION_SUCCEEDED])))
      .catch((e: HttpErrorResponse) => this._snackbarService.handleError(e));
  }

  protected resetPosition(id: number) {
    this._processService
      .resetPosition({
        id,
      })
      .then(() => this.getTranslations().then(values => this._snackbarService.showSuccess(values[TranslationConstants.PROCESS_HOLDER_GROUPS.OPERATION_SUCCEEDED])))
      .catch((e: HttpErrorResponse) => this._snackbarService.handleError(e));
  }

  protected clear() {
    this.filterText.set("");
    this.buildTree(this.processHolderGroups());
  }

  protected filter(event: Event) {
    if (!this.filterText().length) {
      this.buildTree(this.processHolderGroups());
    }
    const filteredResults = this.processHolderGroups().filter(
      (group) =>
        group.name.includes(this.filterText()) ||
        group.positions?.some(
          (x) =>
            x.order?.includes(this.filterText()) ||
            x.activity?.includes(this.filterText()) ||
            x.process?.includes(this.filterText())
        )
    );
    this.buildTree(filteredResults);
  }

  protected disconnectEvents() {
    this._processHolderStreamService.disconnect();
  }
}

