import { CommonModule } from '@angular/common';
import {
  Component,
  computed,
  inject,
  input,
  OnInit,
  resource,
  signal,
} from '@angular/core';
import { MaintenanceFormComponent } from '../maintenance-form/maintenance-form.component';
import { Entry, MoryxSnackbarService } from '@moryx/ngx-web-framework';
import { MaintenanceStoreService } from '../../services/maintenance-store.service';
import { MaintenanceManagementService } from '../../api/services';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router, RouterLink } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { TranslationConstants } from '../../extensions/translation-constants.extensions';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-maintenance',
  imports: [
    CommonModule,
    MaintenanceFormComponent,
    MatProgressSpinnerModule,
    RouterLink,
    MatIconModule,
    MatButtonModule,
    TranslateModule,
  ],
  templateUrl: './maintenance.component.html',
  styleUrl: './maintenance.component.scss',
  providers: [
    MaintenanceStoreService,
    MaintenanceManagementService,
    MoryxSnackbarService,
  ],
})
export class MaintenanceComponent implements OnInit {
  snackbarService = inject(MoryxSnackbarService);

  id = input.required<number | undefined>();

  isNew = computed<boolean>(() => {
    return Number(this.id()) === 0 || Number(this.id()) === undefined;
  });

  maintenanceResource = resource({
    loader: () => {
      if (this.isNew()) return this.maintenanceService.prototype().toAsync();
      return this.maintenanceService.get({ id: this.id() ?? 0 }).toAsync();
    },
  });

  maintenance = computed<Entry | undefined>(() => {
    return this.maintenanceResource.value();
  });

  readonly storeService = inject(MaintenanceStoreService);
  readonly maintenanceService = inject(MaintenanceManagementService);
  readonly router = inject(Router);
  readonly translate = inject(TranslateService);
  TranslationConstants = TranslationConstants;

  ngOnInit(): void {}

  private async getTranslations(): Promise<{ [key: string]: string }> {
    return await this.translate
      .get([TranslationConstants.APP.SNACK_BAR.ADDED])
      .toAsync();
  }

  save() {
    if (this.isNew()) {
      this.maintenanceService
        .add({
          body: this.maintenance(),
        })
        .subscribe({
          next: async (response) => {
            const translations = await this.getTranslations();
            this.snackbarService.showSuccess(
              translations[TranslationConstants.APP.SNACK_BAR.ADDED]
            );
          },
          error: (e) => this.snackbarService.showError(e),
        });
    } else {
      this.maintenanceService
        .update({
          id: this.id() ?? -1,
          body: this.maintenance(),
        })
        .subscribe({
          next: async (response) => {
            const translations = await this.getTranslations();
            this.snackbarService
              .showSuccess(translations[TranslationConstants.APP.SNACK_BAR.ADDED])
              .then(() => {
                this.router.navigate([]);
              });
          },
          error: (e) => this.snackbarService.handleError(e),
        });
    }
  }
}
