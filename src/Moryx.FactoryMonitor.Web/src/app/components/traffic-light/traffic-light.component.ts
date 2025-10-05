import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CellState } from 'src/app/api/models/Moryx/FactoryMonitor/Endpoints/Model/cell-state';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import Cell from 'src/app/models/cell';
import { CellStoreService } from 'src/app/services/cell-store.service';
@Component({
    selector: 'app-traffic-light',
    templateUrl: './traffic-light.component.html',
    styleUrls: ['./traffic-light.component.scss'],
    standalone: true,
    imports: [ CommonModule]
})
export class TrafficLightComponent implements OnInit {
  currentState = signal<CellState | undefined | null>(undefined);
  currentStateString = signal<string | undefined>(undefined);
  private id: number | undefined;
  CellState = CellState;
  TranslationConstants = TranslationConstants;

  constructor(private cellStoreService: CellStoreService, public translate: TranslateService) {}

  ngOnInit(): void {
    this.cellStoreService.cellSelected$.subscribe({
      next: c => {
        this.id = c?.id;
        this.updateState(c);
      },
    });
    this.cellStoreService.cellUpdated$.subscribe(async c => await this.updateState(c));
  }

  async getTranslations(): Promise<{ [key: string]: string }> {
    return await this.translate
      .get([
        TranslationConstants.CELL_DETAILS.IDLE_STATE,
        TranslationConstants.CELL_DETAILS.RUNNING_STATE,
        TranslationConstants.CELL_DETAILS.NOT_READY_TO_WORK_STATE,
      ])
      .toAsync();
  }

  private async updateState(newCellParameters: Cell | undefined): Promise<void> {
    if (!newCellParameters) this.currentState.set(newCellParameters);
    if (newCellParameters?.id != this.id) return;

    this.currentStateString.set(await this.getStringState(newCellParameters?.state ?? CellState.Idle));
    this.currentState.set(newCellParameters?.state);
  }

  public async getStringState(state : CellState) {
    const translations = await this.getTranslations();

    switch (state) {
      case CellState.Idle:
      case CellState.Requested:
        return translations[TranslationConstants.CELL_DETAILS.IDLE_STATE];
      case CellState.NotReadyToWork:
        return translations[TranslationConstants.CELL_DETAILS.NOT_READY_TO_WORK_STATE];
      case CellState.Running:
        return translations[TranslationConstants.CELL_DETAILS.RUNNING_STATE];
      default:
        return translations[TranslationConstants.CELL_DETAILS.IDLE_STATE];
    }
  }
}
