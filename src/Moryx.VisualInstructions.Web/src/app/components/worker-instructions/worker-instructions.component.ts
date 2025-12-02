import { HttpErrorResponse } from "@angular/common/http";
import {
  Component,
  effect,
  Input,
  model,
  OnDestroy,
  OnInit,
  signal,
  untracked,
} from "@angular/core";
import {
  EmptyStateComponent,
  Entry,
  MoryxSnackbarService,
  NavigableEntryEditorComponent,
} from "@moryx/ngx-web-framework";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { ReplaySubject, Subject, Subscription, switchMap } from "rxjs";
import { TranslationConstants } from "src/app/extensions/translation-constants.extensions";
import { environment } from "src/environments/environment";
import {
  InstructionContentType,
  InstructionItemModel,
  InstructionModel,
  InstructionResultModel,
  InstructionType,
} from "../../api/models";
import { VisualInstructionsService } from "../../api/services";
import { InstructionService } from "src/app/services/instruction.service";
import { InstructionResponseModel } from "src/app/api/models/instruction-response-model";
import { DisplayedMediaContent } from "../media-contents/displayed-media-content";
import { CommonModule } from "@angular/common";
import { MatCardModule } from "@angular/material/card";
import { MediaContentsComponent } from "../media-contents/media-contents.component";
import { MatDividerModule } from "@angular/material/divider";
import { MatButtonModule } from "@angular/material/button";
import { MarkdownComponent } from "ngx-markdown";
import { InstructionStateService } from 'src/app/services/instruction-state.service';

@Component({
  selector: "app-worker-instructions",
  templateUrl: "./worker-instructions.component.html",
  styleUrls: ["./worker-instructions.component.scss"],
  imports: [
    CommonModule,
    MatCardModule,
    MediaContentsComponent,
    MatDividerModule,
    NavigableEntryEditorComponent,
    EmptyStateComponent,
    TranslateModule,
    MatButtonModule,
    MatCardModule,
    MarkdownComponent
  ],
  standalone: true,
})
export class WorkerInstructionsComponent implements OnInit, OnDestroy {
  clientIdentifier = model.required<string>();

  instructions = signal<InstructionModel[]>([]);
  activeInstructionIndex = signal(0);
  inputs = signal<Entry | undefined> (undefined);
  mediaItems = signal<InstructionItemModel[]>([]);
  displayedInstruction = signal<InstructionModel | undefined>(undefined);
  mediaItemsContent = signal<DisplayedMediaContent[]>([]);
  textItems = signal<InstructionItemModel[]>([]);

  InstructionType = InstructionType;
  InstructionContentType = InstructionContentType;
  environment = environment;
  TranslationConstants = TranslationConstants;
  
  private activeInstructionIndexChange: Subject<number> = new ReplaySubject<number>(
    1
  );
  private _instructorSubscription?: Subscription;
  private _instructionSubscription?: Subscription;

  constructor(
    private visualInstructionsService: VisualInstructionsService,
    private instructionService: InstructionService,
    public translate: TranslateService,
    private moryxSnackbar: MoryxSnackbarService,
    public instructionStateService: InstructionStateService
  ) {
    effect(() => {
      const identifier = this.clientIdentifier();
      untracked(() => {
        this.initialize(identifier);
      })
    });

    this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
    ]);
  }

  initialize(value: string) {
    this._instructorSubscription?.unsubscribe();
    this._instructorSubscription =
      this.instructionService.instructions$.subscribe(instructions =>this.onInstructionsUpdated(instructions)
      );
  }

  ngOnInit(): void {
    this._instructionSubscription = this.activeInstructionIndexChange
      .pipe(
        switchMap(async (index) => await this.switchInstruction(index)),
        switchMap(async (_) => await this.fetchMediaContents())
      )
      .subscribe((contents) => (this.mediaItemsContent.update(_ => contents)));
  }

  ngOnDestroy(): void {
    this._instructionSubscription?.unsubscribe();
    this._instructorSubscription?.unsubscribe();
  }

  private switchInstruction(index: number): Promise<void> {
    const instruction = this.instructions().length
      ? this.instructions()[index]
      : undefined;
    if (instruction == undefined || Object.keys(instruction).length === 0) {
      this.activeInstructionIndex.update(_ => 0);
      this.clearCurrentView();
      return Promise.resolve();
    }
    this.activeInstructionIndex.update(_ => index);
    if (this.displayedInstruction()?.id === instruction.id)
      return Promise.resolve();

    this.displayedInstruction.update( _ => instruction);
    this.mediaItems.update(_ => instruction.items?.filter(
      (i) => i.contentType == InstructionContentType.Media
    ) ?? []);
    this.textItems.update(_ =>  instruction.items?.filter(
      (i) => i.contentType == InstructionContentType.Text
    ) ?? []);
    this.inputs.update(_ => instruction.inputs!);
    return Promise.resolve();
  }

  private fetchMediaContents(): Promise<DisplayedMediaContent[]> {
    if (!this.mediaItems().length)
      return Promise.resolve<DisplayedMediaContent[]>([]);
    return this.instructionService.requestMediaContentsAsync(this.mediaItems());
  }

  private onInstructionsUpdated(update: InstructionModel[]) {
    this.updateInstructions(update);
    this.updateInstructionIndex();
  }

  private updateInstructionIndex() {
    let updatedIndex = this.instructions().findIndex(
      (i) => i.id === this.displayedInstruction()?.id
    );
    if (updatedIndex < 0 || !this.inputs || !this.inputsChanged(this.inputs()!)) {
      this.activeInstructionIndexChange.next(this.instructions().length - 1);
      return;
    }

    this.activeInstructionIndex.update(_ => updatedIndex);
  }

  private inputsChanged(entry: Entry): boolean {
    if (entry.value.current !== entry.value.default) return true;
    if (!entry.subEntries?.length) return false;
    return entry.subEntries.some((s: any) => this.inputsChanged(s));
  }

  private updateInstructions(update: InstructionModel[]) {
    if (!update.length) {
      this.instructions.update((_) => []);
      return;
    }
    const unchangedInstructions = this.instructions().filter((i) =>
      update.some((nI) => nI.id === i.id)
    );
    const newInstructions = update.filter(
      (nI) => !unchangedInstructions.some((i) => i.id === nI.id)
    );
    this.instructions.update((_) =>
      unchangedInstructions.concat(newInstructions)
    );
  }

  onSwipeLeft(): void {
    const rightIndex =
      (1 + this.activeInstructionIndex()) % this.instructions().length;
    this.activeInstructionIndexChange.next(rightIndex);
  }

  onSwipeRight(): void {
    const leftIndex =
      (this.instructions().length - 1 + this.activeInstructionIndex()) %
      this.instructions().length;
    this.activeInstructionIndexChange.next(leftIndex);
  }

  onSelectResult(result: InstructionResultModel): void {
    const target = this.displayedInstruction()?.id;
    const response = <InstructionResponseModel>{
      id: this.displayedInstruction()?.id,
      inputs: this.inputs(),
      selectedResult: result,
    };
    this.visualInstructionsService
      .completeInstruction$Response({
        identifier: this.clientIdentifier(),
        body:response,
      })
      .subscribe({
        next: () => this.clearCurrentViewOf(target),
        error: async (e: HttpErrorResponse) =>
          await this.moryxSnackbar.handleError(e),
      });
  }

  clearCurrentViewOf(id: number | undefined) {
    if (this.displayedInstruction()?.id === id) this.clearCurrentView();
  }

  clearCurrentView() {
    this.displayedInstruction.update(_ => undefined);
    this.mediaItems.update(_ => []);
    this.mediaItemsContent.update(_ => []);
    this.textItems.update(_ => []);
    this.inputs.update(_ => undefined);
  }

  toggleFullscreen() {
    this.instructionStateService.toggleFullscreen();
  }
}
