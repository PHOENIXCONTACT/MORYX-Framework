import { CommonModule } from '@angular/common';
import {
  Component,
  computed,
  ElementRef,
  inject,
  OnInit,
  signal,
  ViewChild,
} from '@angular/core';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Maintenance, mapFrom } from '../../models/maintenance';
import { MAINTENANCES } from '../../sample-data';
import { IntervalType } from '../../models/interval-base';
import { MatMenuModule } from '@angular/material/menu';
import { MatDrawer, MatSidenavModule } from '@angular/material/sidenav';
import { AcknowledgementsComponent } from '../acknowledgements/acknowledgements.component';
import { RouterLink } from '@angular/router';
import { MaintenanceStoreService } from '../../services/maintenance-store.service';
import {
  EmptyStateComponent,
  MoryxSnackbarService,
} from '@moryx/ngx-web-framework';
import { MaintenanceManagementService } from '../../api/services';
import { MaintenanceStreamService } from '../../services/maintenance-stream.service';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from '../../dialogs/confirmation-dialog/confirmation-dialog.component';
import { MatChipListboxChange, MatChipsModule } from '@angular/material/chips';
import { TranslationConstants } from '../../extensions/translation-constants.extensions';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-maintenances',
  imports: [
    CommonModule,
    MatGridListModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatSidenavModule,
    AcknowledgementsComponent,
    RouterLink,
    EmptyStateComponent,
    MatChipsModule,
    TranslateModule,
  ],
  templateUrl: './maintenances.component.html',
  styleUrl: './maintenances.component.scss',
  providers: [
    MaintenanceStoreService,
    MoryxSnackbarService,
    MaintenanceStreamService,
    TranslateService
  ],
})
export class MaintenancesComponent implements OnInit {
  readonly maintenances = signal<Maintenance[]>([]);
  readonly filteredMaintenances = computed<Maintenance[]>(() => {
    const currentFilter = this.selectedFilter();
    const values = this.maintenances();
    switch (currentFilter) {
      case 'All':
        return values;
      case 'Active':
        return values.filter((x) => x.isActive);
      case 'Inactive':
        return values.filter((x) => !x.isActive);
      default:
        return [];
    }
  });
  readonly selectedMaintenance = signal<Maintenance | undefined>(undefined);
  readonly selectedFilter = signal<FilterType>('Active');

  readonly storeService = inject(MaintenanceStoreService);
  readonly maintenanceService = inject(MaintenanceManagementService);
  readonly snackbarService = inject(MoryxSnackbarService);
  readonly streamService = inject(MaintenanceStreamService);
  readonly dialog = inject(MatDialog);
  readonly translate = inject(TranslateService);

  readonly IntervalType = IntervalType;
  TranslationConstants = TranslationConstants;

  ngOnInit(): void {
    this.maintenanceService.getAll_2().subscribe({
      next: (response) => {
        this.maintenances.set(response.data?.map((x) => mapFrom(x)) ?? []);
      },
      error: (e) => this.snackbarService.handleError(e),
    });

    this.streamService.$updateMaintenanceOrder.subscribe({
      next: (value) => {
        this.maintenances.update((items) => {
          const match = items.find((X) => X.id === value?.id);
          if (match) {
            Object.assign(match, value);
          }
          return items;
        });
      },
    });
  }

  private async getTranslations(): Promise<{ [key: string]: string }> {
    return await this.translate
      .get([
        TranslationConstants.APP.SNACK_BAR.ADDED,
        TranslationConstants.APP.SNACK_BAR.REMOVED,
        TranslationConstants.APP.SNACK_BAR.START_SENT,
        TranslationConstants.INTERVALS.CYCLES,
        TranslationConstants.INTERVALS.DAYS,
        TranslationConstants.INTERVALS.HOURS,
      ])
      .toAsync();
  }

  onSelectedFilterChange(event: MatChipListboxChange) {
    this.selectedFilter.set(event.value);
  }

  onConfirmDelete(id: number) {
    //show confirmation
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '350px',
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.delete(id);
      }
    });
  }

  delete(id: number) {
    this.maintenanceService
      .delete({
        id,
      })
      .subscribe({
        next: async (response) => {
          const translations = await this.getTranslations();
          this.snackbarService.showSuccess(
            translations[TranslationConstants.APP.SNACK_BAR.REMOVED]
          );
        },
        error: (e) => this.snackbarService.handleError(e),
      });
  }

  select(item: Maintenance) {
    this.selectedMaintenance.set(item);
  }

  startMaintenance(maintenance: Maintenance) {
    this.maintenanceService
      .start({
        id: maintenance.id,
      })
      .subscribe({
        next: async (response) => {
          const translations = await this.getTranslations();
          this.snackbarService.showSuccess(
            translations[TranslationConstants.APP.SNACK_BAR.START_SENT]
          );
        },
        error: (e) => this.snackbarService.handleError(e),
      });
  }

  onMaintenanceCardClick(maintenance: Maintenance, drawer: MatDrawer) {
    if (drawer.opened) this.select(maintenance);
  }

  getIntervalString(type: IntervalType) {
    const interval = type.toString();
    switch (interval) {
      case IntervalType[IntervalType.Day]:
        return TranslationConstants.INTERVALS.DAYS ;
      case IntervalType[IntervalType.Cycle]:
        return TranslationConstants.INTERVALS.CYCLES ;
      case IntervalType[IntervalType.Hour]:
        return TranslationConstants.INTERVALS.HOURS ;
      default:
        return "?";
    }
  }
}
export type FilterType = 'All' | 'Active' | 'Inactive';
