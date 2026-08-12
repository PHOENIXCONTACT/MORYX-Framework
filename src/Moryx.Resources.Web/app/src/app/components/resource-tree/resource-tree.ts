/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, viewChild, input, output, effect, untracked, ChangeDetectionStrategy } from '@angular/core';
import { MatTree, MatTreeModule } from '@angular/material/tree';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { TranslatePipe } from '@ngx-translate/core';
import { ResourceModel } from '../../api/models';
import { SessionService } from '@app/services/session.service';
import { TranslationConstants } from '@app/extensions/translation-constants';
import { getHierarchieLineFor } from '@app/models/TypeTree';

@Component({
  selector: 'app-resource-tree',
  templateUrl: './resource-tree.html',
  styleUrls: ['./resource-tree.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [MatTreeModule, MatIconModule, MatButtonModule, TranslatePipe]
})
export class ResourceTree {
  private sessionService = inject(SessionService);
  private expandedIds = new Set<number>();

  readonly resources = input.required<ResourceModel[]>();
  readonly selected = input<ResourceModel | undefined>(undefined);

  readonly nodeSelected = output<number>();
  readonly nodeContextMenu = output<{ event: MouseEvent; id: number }>();

  protected readonly tree = viewChild<MatTree<ResourceModel>>(MatTree);

  protected childrenAccessor = (node: ResourceModel) =>
    (node.references?.find(ref => ref.name === 'Children')?.targets ?? []) as ResourceModel[];

  protected hasChild = (_: number, node: ResourceModel) =>
    !!(node.references?.find(ref => ref.name === 'Children')?.targets?.length);

  protected TranslationConstants = TranslationConstants;

  constructor() {
    // Restore expanded tree state from session when resources are loaded
    effect(() => {
      const data = this.resources();
      const tree = this.tree();
      if (!tree || !data.length) {
        return;
      }

      untracked(() => {
        const sessionIds = this.sessionService.getExpandedIds();
        for (const id of sessionIds) {
          this.expandedIds.add(id);
        }
        this.expandToSelected();
        this.applyExpandedState(data);
        this.storeState();
      });
    });

    // Expand nodes to make the selected resource visible in the tree
    effect(() => {
      const selectedId = this.selected()?.id;
      if (selectedId) {
        untracked(() => {
          this.expandToSelected();
          this.applyExpandedState(this.resources());
          this.storeState();
        });
      }
    });
  }

  protected onNodeClick(id: number) {
    this.nodeSelected.emit(id);
  }

  protected onContextMenu(event: MouseEvent, id: number) {
    this.nodeContextMenu.emit({ event, id });
  }

  protected onExpandOrCollapseNode(node: ResourceModel) {
    if (this.expandedIds.has(node.id!)) {
      this.expandedIds.delete(node.id!);
    } else {
      this.expandedIds.add(node.id!);
    }
    this.storeState();
  }

  private storeState() {
    this.sessionService.storeTreeState([...this.expandedIds]);
  }

  private expandToSelected() {
    const data = this.resources();
    const selectedId = this.selected()?.id;
    if (!data.length || !selectedId) {
      return;
    }
    const toExpand = getHierarchieLineFor(selectedId, data);
    for (const id of toExpand) {
      this.expandedIds.add(id);
    }
  }

  private applyExpandedState(nodes: ResourceModel[]) {
    for (const node of nodes) {
      if (this.expandedIds.has(node.id!)) {
        this.tree()!.expand(node);
      }
      this.applyExpandedState(this.childrenAccessor(node));
    }
  }
}
