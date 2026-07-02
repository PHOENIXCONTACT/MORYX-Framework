/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { Component, inject, viewChild, input, output, effect, ChangeDetectionStrategy } from '@angular/core';
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
    // Re-expand saved nodes whenever treeData or tree instance changes
    effect(() => {
      const data = this.treeData();
      const tree = this.tree();
      if (!tree) {
        return;
      }
      const names = this.sessionService.getExpandedNodeNames();
      this.expandSavedNodes(data, names);
    });
  }

  protected onNodeClick(id: number) {
    this.nodeSelected.emit(id);
  }

  protected onContextMenu(event: MouseEvent, id: number) {
    this.nodeContextMenu.emit({ event, id });
  }

  protected onExpandOrCollapseNode(node: ProductNode) {
    this.sessionService.saveProductTreeExpansion(node, this.tree()!.isExpanded(node));
  }

  protected createProductIdentity(identifier: string | undefined | null, revision: number | undefined): string {
    return this.editProductsService.createProductIdentity(identifier, revision);
  }

  private expandSavedNodes(nodes: ProductNode[], expandedNames: string[]) {
    for (const node of nodes) {
      if (expandedNames.includes(node.name)) {
        this.tree()!.expand(node);
      }
      if (node.children) {
        this.expandSavedNodes(node.children, expandedNames);
      }
    }
  }
}
