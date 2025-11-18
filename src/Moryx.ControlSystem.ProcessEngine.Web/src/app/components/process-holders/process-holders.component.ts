import {
  Component,
  ElementRef,
  inject,
  OnInit,
  signal,
  viewChild,
} from "@angular/core";
import { CommonModule } from "@angular/common";
import { MatTree, MatTreeModule } from "@angular/material/tree";
import { MatIconModule } from "@angular/material/icon";
import { MatButtonModule } from "@angular/material/button";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { ProcessHolderStreamService } from "../../services/process-holder-stream.service";
import { MatTooltipModule } from "@angular/material/tooltip";
import { HttpErrorResponse } from "@angular/common/module.d-CnjH8Dlt";
import {
  EmptyStateComponent,
  MoryxSnackbarService,
} from "@moryx/ngx-web-framework";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { ProcessEngineService } from "src/app/api/services";
import { ProcessHolderGroup } from "src/app/models/process-holder-group-model";
import ProcessHolderNode from "../../models/process-holder-node";
import { Category } from "src/app/api/models/category";
import { ConvertToNode, ConvertToProcessHolderGroup } from "src/app/models/converter";
import { ProcessHolderGroupModelArrayApiResponse } from "src/app/api/models/process-holder-group-model-array-api-response";
import { ProcessHolderGroupModel } from "src/app/api/models/process-holder-group-model";

@Component({
  selector: "app-process-holders",
  imports: [
    CommonModule,
    MatTreeModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    EmptyStateComponent,
    MatFormFieldModule,
    ReactiveFormsModule,
    FormsModule,
    MatIconModule,
    MatInputModule,
    TranslateModule
  ],
  templateUrl: "./process-holders.component.html",
  styleUrl: "./process-holders.component.scss",
})
export class ProcessHoldersComponent implements OnInit {
  processHolderGroups = signal<Array<ProcessHolderGroup>>([]);
  dataSource = signal<Array<ProcessHolderNode>>([]);
  loading = signal(false);
  filterText = signal("");
  visualizationCategory = Category;

  TranslationConstants = TranslationConstants;

  hasChild = (_: number, node: ProcessHolderNode) =>
    !!node.children && node.children.length > 0;
  childrenAccessor = (node: ProcessHolderNode) => node.children ?? [];

  private _processHolderStreamService = inject(ProcessHolderStreamService);
  private _processService = inject(ProcessEngineService);
  private _moryxSnackbar = inject(MoryxSnackbarService);
  private _translate = inject(TranslateService);

  private _tree = viewChild<MatTree<ProcessHolderNode, ProcessHolderNode>>("tree");

  ngOnInit(): void {
    this.loading.set(true);
    this._processService.getGroups().subscribe({
      next: (response: ProcessHolderGroupModelArrayApiResponse) => {
        console.log(response);
        this.processHolderGroups.set(response.data?.map((x) => ConvertToProcessHolderGroup(x)) ?? []);
        this.buildTree(this.processHolderGroups());
        this.loading.set(false);

        this._processHolderStreamService.$updatedWpc.subscribe((group) => {
          if (group) {
            this.updateTree(ConvertToProcessHolderGroup(group));
          }
        });
      },
      error: (e: HttpErrorResponse) => {
        this.loading.set(false);
        this.processHolderGroups.set([]);
        this.buildTree([]);
        this._moryxSnackbar.handleError(e);
      },
    });
  }

  private async getTranslations(): Promise<{ [key: string]: string }> {
    return await this._translate
      .get([TranslationConstants.PROCESS_HOLDER_GROUPS.OPERATION_SUCCEEDED])
      .toAsync();
  }


  updateTree(group: ProcessHolderGroup) {
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
      } else nodes.push(node);
      return nodes;
    });
  }

  buildTree(groups: ProcessHolderGroup[]) {
    const nodes: ProcessHolderNode[] = [];
    for (const group of groups) {
      const node = ConvertToNode(group);
      nodes.push(node);
    }
    this.dataSource.set(nodes);
  }

  resetGroup(id: number) {
    this._processService
      .resetGroup({
        id,
      })
      .subscribe({
        next: () => this.getTranslations().then(values => this._moryxSnackbar.showSuccess(values[TranslationConstants.PROCESS_HOLDER_GROUPS.OPERATION_SUCCEEDED])),
        error: (e: HttpErrorResponse) => this._moryxSnackbar.handleError(e),
      });
  }

  resetPosition(id: number) {
    this._processService
      .resetPosition({
        id,
      })
      .subscribe({
        next: () => this.getTranslations().then(values => this._moryxSnackbar.showSuccess(values[TranslationConstants.PROCESS_HOLDER_GROUPS.OPERATION_SUCCEEDED])),
        error: (e: HttpErrorResponse) => this._moryxSnackbar.handleError(e),
      });
  }

  clear() {
    this.filterText.set("");
    this.buildTree(this.processHolderGroups());
  }

  filter(event: Event) {
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
}
