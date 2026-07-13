/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { HttpErrorResponse } from "@angular/common/http";
import { Component, effect, inject, model, signal, untracked, ChangeDetectionStrategy } from "@angular/core";
import { EmptyState } from "@moryx/ngx-web-framework/empty-state";
import { Entry, NavigableEntryEditor } from "@moryx/ngx-web-framework/entry-editor";
import { SnackbarService } from "@moryx/ngx-web-framework/services";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { TranslationConstants } from "@app/extensions/translation-constants.extensions";
import { environment } from "../../../environments/environment";
import {
  InstructionContentType,
  InstructionItemModel,
  InstructionModel,
  InstructionResultModel,
  InstructionType,
} from "@api/models";
import { VisualInstructionsService } from "@api/services";
import { InstructionService } from "@app/services/instruction.service";
import { InstructionResponseModel } from "@app/api/models/instruction-response-model";
import { DisplayedMediaContent } from "../media-contents/displayed-media-content";

import { MatCardModule } from "@angular/material/card";
import { MediaContents } from "../media-contents/media-contents";
import { MatDividerModule } from "@angular/material/divider";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { MarkdownComponent } from "ngx-markdown";
import { InstructionStateService } from '@app/services/instruction-state.service';

@Component({
  selector: "app-worker-instructions",
  templateUrl: "./worker-instructions.html",
  styleUrls: ["./worker-instructions.scss"],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    MatCardModule,
    MediaContents,
    MatDividerModule,
    NavigableEntryEditor,
    EmptyState,
    TranslatePipe,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MarkdownComponent
  ]
})
export class WorkerInstructions {
  private visualInstructionsService = inject(VisualInstructionsService);
  private instructionService = inject(InstructionService);
  private translateService = inject(TranslateService);
  private snackbarService = inject(SnackbarService);
  private instructionStateService = inject(InstructionStateService);

  readonly clientIdentifier = model.required<string>();
  protected fullscreen = this.instructionStateService.fullscreen;

  protected instructions = signal<InstructionModel[]>([]);
  protected activeInstructionIndex = signal(0);
  protected inputs = signal<Entry | undefined>(undefined);
  protected mediaItems = signal<InstructionItemModel[]>([]);
  protected displayedInstruction = signal<InstructionModel | undefined>(undefined);
  protected mediaItemsContent = signal<DisplayedMediaContent[]>([]);
  protected textItems = signal<InstructionItemModel[]>([]);

  protected InstructionType = InstructionType;
  protected InstructionContentType = InstructionContentType;
  protected environment = environment;
  protected TranslationConstants = TranslationConstants;

  constructor() {
    effect(() => {
      const instructions = this.instructionService.instructions();
      untracked(() => {
        this.onInstructionsUpdated(instructions);
      });
    });

    this.translateService.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
  }

  private async onIndexChange(index: number) {
    this.switchInstruction(index);
    const contents = await this.fetchMediaContents();
    this.mediaItemsContent.set(contents);
  }

  private switchInstruction(index: number) {
    const instruction = this.instructions()[index];
    if (!instruction || Object.keys(instruction).length === 0) {
      this.activeInstructionIndex.set(0);
      this.clearCurrentView();
      return;
    }
    this.activeInstructionIndex.set(index);
    if (this.displayedInstruction()?.id === instruction.id) {
      return;
    }

    this.displayedInstruction.set(instruction);
    this.mediaItems.set(instruction.items?.filter(
      (i) => i.contentType == InstructionContentType.Media
    ) ?? []);
    this.textItems.set(instruction.items?.filter(
      (i) => i.contentType == InstructionContentType.Text
    ) ?? []);
    this.inputs.set(instruction.inputs);
  }

  private async fetchMediaContents(): Promise<DisplayedMediaContent[]> {
    if (!this.mediaItems().length) {
      return [];
    }
    return await this.instructionService.requestMediaContentsAsync(this.mediaItems());
  }

  private onInstructionsUpdated(update: InstructionModel[]) {
    this.updateInstructions(update);
    this.updateInstructionIndex();
  }

  private updateInstructionIndex() {
    const updatedIndex = this.instructions().findIndex(
      (i) => i.id === this.displayedInstruction()?.id
    );
    if (updatedIndex < 0 || !this.inputs || !this.inputsChanged(this.inputs())) {
      this.onIndexChange(this.instructions().length - 1);
      return;
    }

    this.activeInstructionIndex.set(updatedIndex);
  }

  private inputsChanged(entry: Entry | undefined): boolean {
    if (entry?.value.current !== entry?.value.default) {
      return true;
    }
    if (!entry?.subEntries?.length) {
      return false;
    }
    return entry.subEntries.some((s: Entry) => this.inputsChanged(s));
  }

  private updateInstructions(update: InstructionModel[]) {
    if (!update.length) {
      this.instructions.set([]);
      return;
    }
    const unchangedInstructions = this.instructions().filter((i) =>
      update.some((nI) => nI.id === i.id)
    );
    const newInstructions = update.filter(
      (nI) => !unchangedInstructions.some((i) => i.id === nI.id)
    );
    this.instructions.set(unchangedInstructions.concat(newInstructions));
  }

  protected async onSwipeLeft() {
    const rightIndex =
      (1 + this.activeInstructionIndex()) % this.instructions().length;
    await this.onIndexChange(rightIndex);
  }

  protected async onSwipeRight() {
    const leftIndex =
      (this.instructions().length - 1 + this.activeInstructionIndex()) %
      this.instructions().length;
    await this.onIndexChange(leftIndex);
  }

  protected onSelectResult(result: InstructionResultModel): void {
    const target = this.displayedInstruction()?.id;
    const response = <InstructionResponseModel>{
      id: this.displayedInstruction()?.id,
      inputs: this.inputs(),
      selectedResult: result,
    };
    this.visualInstructionsService
      .completeInstruction$Response({
        identifier: this.clientIdentifier(),
        body: response
      })
      .then(() => this.clearCurrentViewOf(target))
      .catch((e: HttpErrorResponse) => this.snackbarService.handleError(e));
  }

  private clearCurrentViewOf(id: number | undefined) {
    if (this.displayedInstruction()?.id === id) {
      this.clearCurrentView();
    }
  }

  private clearCurrentView() {
    this.displayedInstruction.set(undefined);
    this.mediaItems.set([]);
    this.mediaItemsContent.set([]);
    this.textItems.set([]);
    this.inputs.set(undefined);
  }

  protected toggleFullscreen() {
    this.instructionStateService.toggleFullscreen();
  }
}

