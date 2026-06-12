/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, viewChild, input, output, effect, untracked, ChangeDetectionStrategy, HostListener } from '@angular/core';
import { MatTree, MatTreeModule } from '@angular/material/tree';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { ResourceModel } from '../../api/models';
import { SessionService } from '@app/services/session.service';
import { TranslationConstants } from '@app/extensions/translation-constants.extensions';
import { getHierarchieLineFor } from '@app/models/TypeTree';

@Component({
  selector: 'app-resource-tree',
  templateUrl: './resource-tree.html',
  styleUrls: ['./resource-tree.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [CommonModule, MatTreeModule, MatIconModule, MatButtonModule, TranslateModule]
})
export class ResourceTree {
  private sessionService = inject(SessionService);

  resources = input.required<ResourceModel[]>();
  selected = input<ResourceModel | undefined>(undefined);

  nodeSelected = output<number>();
  nodeContextMenu = output<{ event: MouseEvent; id: number }>();

  tree = viewChild<MatTree<ResourceModel>>(MatTree);

  childrenAccessor = (node: ResourceModel) =>
    (node.references?.find(ref => ref.name === 'Children')?.targets ?? []) as ResourceModel[];

  hasChild = (_: number, node: ResourceModel) =>
    !!(node.references?.find(ref => ref.name === 'Children')?.targets?.length);

  TranslationConstants = TranslationConstants;

  constructor() {
    effect(() => {
      const data = this.resources();
      const tree = this.tree();
      if (!tree || !data.length) {
        return;
      }

      const expandedIds = this.sessionService.getExpandedIds();
      if (expandedIds.length > 0) {
        this.restoreExpandedNodes(data, expandedIds);
      } else {
        untracked(() => {
          const selectedId = this.selected()?.id;
          if (selectedId) {
            const toExpand = getHierarchieLineFor(selectedId, data);
            this.expandNodesById(data, toExpand);
            this.sessionService.storeTreeState(this.getExpandedIds());
          }
        });
      }
    });
  }

  @HostListener('window:beforeunload')
  saveState() {
    this.sessionService.storeTreeState(this.getExpandedIds());
  }

  onNodeClick(id: number) {
    this.nodeSelected.emit(id);
  }

  onContextMenu(event: MouseEvent, id: number) {
    this.nodeContextMenu.emit({ event, id });
  }

  onExpandOrCollapseNode() {
    this.sessionService.storeTreeState(this.getExpandedIds());
  }

  private getExpandedIds(): number[] {
    const ids: number[] = [];
    this.collectExpandedIds(this.resources(), ids);
    return ids;
  }

  private collectExpandedIds(nodes: ResourceModel[], ids: number[]) {
    for (const node of nodes) {
      if (this.tree()!.isExpanded(node)) {
        ids.push(node.id!);
      }
      this.collectExpandedIds(this.childrenAccessor(node), ids);
    }
  }

  private restoreExpandedNodes(nodes: ResourceModel[], expandedIds: number[]) {
    for (const node of nodes) {
      if (expandedIds.includes(node.id!)) {
        this.tree()!.expand(node);
      }
      this.restoreExpandedNodes(this.childrenAccessor(node), expandedIds);
    }
  }

  private expandNodesById(nodes: ResourceModel[], ids: (number | undefined)[]) {
    for (const node of nodes) {
      if (ids.includes(node.id)) {
        this.tree()!.expand(node);
      }
      this.expandNodesById(this.childrenAccessor(node), ids);
    }
  }
}
