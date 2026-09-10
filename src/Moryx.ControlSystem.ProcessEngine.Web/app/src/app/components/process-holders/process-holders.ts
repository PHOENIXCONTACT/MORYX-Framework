/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, computed, effect, inject, OnInit, signal, untracked, viewChild, ChangeDetectionStrategy, DestroyRef } from "@angular/core";

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
import { TranslationConstants } from "@app/translation-constants";
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
  private processHolderStreamService = inject(ProcessHolderStreamService);
  private processEngineService = inject(ProcessEngineService);
  private snackbarService = inject(SnackbarService);
  private translateService = inject(TranslateService);

  private readonly tree = viewChild<MatTree<ProcessHolderNode, ProcessHolderNode>>("tree");

  protected processHolderGroups = signal<ProcessHolderGroup[]>([]);
  protected loading = signal(false);
  protected filterText = signal("");
  protected dataSource = computed(() => {
    const groups = this.processHolderGroups();
    const filter = this.filterText();

    if (!filter.length) {
      return groups.map(g => ConvertToNode(g));
    }

    return groups
      .filter(group => this.matchesFilter(group, filter))
      .map(g => ConvertToNode(g));
  });

  protected visualizationCategory = Category;
  protected TranslationConstants = TranslationConstants;

  protected hasChild = (_: number, node: ProcessHolderNode) =>
    !!node.children && node.children.length > 0;
  protected childrenAccessor = (node: ProcessHolderNode) => node.children ?? [];

  constructor() {
    this.destroyRef.onDestroy(() => this.disconnectEvents());

    effect(() => {
      const group = this.processHolderStreamService.updatedProcessHolderGroups();
      untracked(() => {
        if (group) {
          const converted = ConvertToProcessHolderGroup(group);
          const expandedIds = this.getExpandedNodeIds();

          // Replace existing group or append new one
          this.processHolderGroups.update(current => {
            const index = current.findIndex(g => g.id === converted.id);
            if (index >= 0) {
              return current.map((g, i) => i === index ? converted : g);
            }
            return [...current, converted];
          });

          this.expandNodesByIds(expandedIds);
        }
      });
    });

    effect(() => {
      const filter = this.filterText();
      untracked(() => {
        if (filter.length) {
          this.expandAllNodes();
        }
      });
    });
  }

  ngOnInit(): void {
    this.loading.set(true);
    this.processHolderStreamService.connect();
    this.processEngineService.getGroups()
      .then((response: ProcessHolderGroupModelArrayApiResponse) => {
        this.processHolderGroups.set(response.data?.map((x) => ConvertToProcessHolderGroup(x)) ?? []);
        this.loading.set(false);
      })
      .catch((e: HttpErrorResponse) => {
        this.loading.set(false);
        this.processHolderGroups.set([]);
        this.snackbarService.handleError(e);
      });
  }

  private async getTranslations(): Promise<{ [key: string]: string }> {
    return await firstValueFrom(this.translateService
      .get([TranslationConstants.PROCESS_HOLDER_GROUPS.OPERATION_SUCCEEDED]));
  }

  protected resetGroup(id: number) {
    this.processEngineService
      .resetGroup({
        id,
      })
      .then(() => this.getTranslations().then(values => this.snackbarService.showSuccess(values[TranslationConstants.PROCESS_HOLDER_GROUPS.OPERATION_SUCCEEDED])))
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
  }

  protected resetPosition(id: number) {
    this.processEngineService
      .resetPosition({
        id,
      })
      .then(() => this.getTranslations().then(values => this.snackbarService.showSuccess(values[TranslationConstants.PROCESS_HOLDER_GROUPS.OPERATION_SUCCEEDED])))
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
  }

  protected clear() {
    this.filterText.set("");
  }

  protected disconnectEvents() {
    this.processHolderStreamService.disconnect();
  }

  private getExpandedNodeIds(): Set<number> {
    const ids = new Set<number>();
    const tree = this.tree();
    if (tree) {
      for (const node of this.dataSource()) {
        if (tree.isExpanded(node)) {
          ids.add(node.data.id);
        }
      }
    }
    return ids;
  }

  private expandNodesByIds(ids: Set<number>) {
    const tree = this.tree();
    if (tree) {
      for (const node of this.dataSource()) {
        if (ids.has(node.data.id)) {
          tree.expand(node);
        }
      }
    }
  }

  private expandAllNodes() {
    const tree = this.tree();
    if (tree) {
      for (const node of this.dataSource()) {
        tree.expand(node);
      }
    }
  }

  private matchesFilter(group: ProcessHolderGroup, filter: string): boolean {
    return group.name.includes(filter) ||
      !!group.positions?.some(x =>
        x.order?.includes(filter) ||
        x.activity?.includes(filter) ||
        x.process?.includes(filter)
      );
  }
}
