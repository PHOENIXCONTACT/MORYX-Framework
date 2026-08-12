/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, viewChild, input, output, effect, untracked, ChangeDetectionStrategy } from '@angular/core';
import { MatTree, MatTreeModule } from '@angular/material/tree';
import { MatIconModule } from '@angular/material/icon';
import { ProductModel } from '@api/models';
import { EditProductsService } from '@app/services/edit-products.service';
import { SessionService } from '@app/services/session.service';
import { ProductNode } from '@app/app';
import { MatIconButton } from '@angular/material/button';

@Component({
  selector: 'app-product-tree',
  templateUrl: './product-tree.html',
  styleUrls: ['./product-tree.scss'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [MatTreeModule, MatIconModule, MatIconButton]
})
export class ProductTree {
  private sessionService = inject(SessionService);
  private editProductsService = inject(EditProductsService);
  private expandedNames = new Set<string>();

  // Inputs
  readonly treeData = input.required<ProductNode[]>();
  readonly selected = input<ProductModel | undefined>(undefined);

  // Outputs
  readonly nodeSelected = output<number>();
  readonly nodeContextMenu = output<{ event: MouseEvent; id: number }>();

  // Tree internals
  protected readonly tree = viewChild<MatTree<ProductNode>>(MatTree);

  protected childrenAccessor = (node: ProductNode) => node.children ?? [];
  protected hasChild = (_: number, node: ProductNode) => !!node.children?.length;

  constructor() {
    // Restore expanded tree state from session when tree data is loaded
    effect(() => {
      const data = this.treeData();
      const tree = this.tree();
      if (!tree) {
        return;
      }

      untracked(() => {
        for (const name of this.sessionService.getExpandedNodeNames()) {
          this.expandedNames.add(name);
        }
        this.expandToSelected();
        this.applyExpandedState(data);
        this.storeState();
      });
    });

    // Expand parent nodes to make the selected product visible in the tree
    effect(() => {
      const selectedId = this.selected()?.id;
      if (selectedId) {
        untracked(() => {
          this.expandToSelected();
          this.applyExpandedState(this.treeData());
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

  protected onExpandOrCollapseNode(node: ProductNode) {
    if (this.expandedNames.has(node.name)) {
      this.expandedNames.delete(node.name);
    } else {
      this.expandedNames.add(node.name);
    }
    this.storeState();
  }

  protected createProductIdentity(identifier: string | undefined | null, revision: number | undefined): string {
    return this.editProductsService.createProductIdentity(identifier, revision);
  }

  private storeState() {
    this.sessionService.storeProductTreeExpansion([...this.expandedNames]);
  }

  private expandToSelected() {
    const data = this.treeData();
    const selectedId = this.selected()?.id;
    if (!data.length || !selectedId) {
      return;
    }
    for (const name of this.getAncestorNames(selectedId, data)) {
      this.expandedNames.add(name);
    }
  }

  private getAncestorNames(targetId: number, nodes: ProductNode[]): string[] {
    for (const node of nodes) {
      if (node.id === targetId) {
        return [];
      }
      if (node.children) {
        const path = this.getAncestorNames(targetId, node.children);
        if (path !== undefined) {
          path.push(node.name);
          return path;
        }
      }
    }
    return undefined!;
  }

  private applyExpandedState(nodes: ProductNode[]) {
    for (const node of nodes) {
      if (this.expandedNames.has(node.name)) {
        this.tree()!.expand(node);
      }
      if (node.children) {
        this.applyExpandedState(node.children);
      }
    }
  }
}
