import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, OnDestroy, OnInit, signal, viewChild, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuModule, MatMenuTrigger } from '@angular/material/menu';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import {
  EmptyStateComponent,
  LanguageService,
  MoryxSnackbarService
} from '@moryx/ngx-web-framework';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { environment } from 'src/environments/environment';
import {
  ChangedDashboardInformation,
  DashboardInformation
} from './api/models';
import { AnalyticsService } from './api/services';
import { DialogAddDashboardComponent } from './dialogs/dialog-add-dashboard/dialog-add-dashboard.component';
import { DialogEditDashboardComponent } from './dialogs/dialog-edit-dashboard/dialog-edit-dashboard.component';
import { DialogRemoveDashboardsComponent } from './dialogs/dialog-remove-dashboards/dialog-remove-dashboards.component';
import './extensions/observable.extensions';
import { TranslationConstants } from './extensions/translation-constants.extensions';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatListModule } from '@angular/material/list';
import { CommonModule } from '@angular/common';
import { MatDrawer, MatDrawerContainer, MatDrawerContent } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: true,
  imports: [
    EmptyStateComponent,
    MatProgressSpinnerModule,
    TranslateModule,
    MatListModule,
    MatMenuModule,
    CommonModule,
    MatDrawer,
    MatDrawerContent,
    MatIconModule,
    MatButtonModule,
    MatToolbarModule,
    MatDrawerContainer
  ]
})
export class AppComponent implements OnInit, OnDestroy {
  trigger = viewChild.required(MatMenuTrigger);
  menuTopLeftPosition = signal<{ x: String, y: String}>({ x: '0', y: '0' });
  isLoading= signal(true);
  selectedUrl = signal<string | undefined>(undefined);
  dashboards = signal<DashboardInformation[]>([]);
  selectedSafeUrl = computed((): SafeResourceUrl | undefined => {
    const url = this.selectedUrl();
    if (!url) return undefined;
    return this.sanitizer.bypassSecurityTrustResourceUrl(url);
  });
  TranslationConstants = TranslationConstants;

  title = 'Moryx.Analytics.Web';
  analyticsToolbarImage: string =
    environment.assets + 'assets/analytics_toolbar.jpg';

  constructor(
    private sanitizer: DomSanitizer,
    private dialog: MatDialog,
    private languageService: LanguageService,
    private snackBar: MatSnackBar,
    private service: AnalyticsService,
    public translate: TranslateService,
    private moryxSnackbar: MoryxSnackbarService
  ) {
    this.translate.addLangs([
      TranslationConstants.LANGUAGES.EN,
      TranslationConstants.LANGUAGES.DE,
      TranslationConstants.LANGUAGES.IT,
      TranslationConstants.LANGUAGES.ZH
    ]);
    this.translate.setDefaultLang('en');
    this.translate.use(this.languageService.getDefaultLanguage());
  }

  ngOnInit(): void {
    this.service.getAllDashboards().subscribe({
      next: (dashboards) => {
        this.dashboards.update(_=> dashboards);
        this.isLoading.update(_=> false);

        const last = localStorage.getItem('lastSelectedDashboard');
        if (last && dashboards.find(d => d.url === last)) {
          this.selectedUrl.set(last);
        }
      },
      error: async (err) => {
        const translations = await this.translate
          .get([TranslationConstants.APP.FAILED_LOADING])
          .toAsync();
        await this.moryxSnackbar.showError(
          translations[TranslationConstants.APP.FAILED_LOADING]
        );
        this.isLoading.update(_=> false);
      }
    });

    this.translate.get([TranslationConstants.APP.TITLE]).subscribe((title) => {
      this.title = title;
    });
  }

  ngOnDestroy(): void {}

  onSelect(url: string | undefined | null) {
    if (!url) return;
    if (url === this.selectedUrl()) return;

    this.selectedUrl.set(url);
    localStorage.setItem('lastSelectedDashboard', url);
  }

  onAnalyseContext(event: MouseEvent, url: string | undefined | null) {
    event.preventDefault();
    this.open(event.clientX, event.clientY, url);
  }

  open(x: number, y: number, url: string | undefined | null){
    if (!url) return;

    this.trigger().menuData = { url: url };
    this.menuTopLeftPosition.update(_=> <{x: String, y: String}>{x:`${x}px`, y:`${y}px`});
    this.trigger().openMenu();
  }

  onOpenContextMenuOnTouch(event: any, url: string | undefined | null){
    this.open(event.pointers[0].clientX, event.pointers[0].clientY, url);
  }

  async onAdd() {
    const dialogRef = this.dialog.open(DialogAddDashboardComponent, {});
    const dashboard = await dialogRef.afterClosed().toAsync();
    if (!dashboard || !dashboard.url) return;

    await this.service
      .addDashboard({ body: dashboard })
      .toAsync()
      .catch(
        async (e: HttpErrorResponse) =>
          await this.moryxSnackbar.showError(e.error.title)
      );
    this.dashboards.update(items =>{
      items.push(dashboard);
      return items;
    });

    this.selectedUrl.set(dashboard.url);
    localStorage.setItem('lastSelectedDashboard', dashboard.url);
  }

  async onDelete(url: string) {
    const dashboard = this.dashboards()?.find((d) => d.url == url);

    if (!dashboard || !dashboard.name) return;
    const dialogRef = this.dialog.open(DialogRemoveDashboardsComponent, {
      data: dashboard?.name,
    });

    const result = await dialogRef.afterClosed().toAsync();
    if (!result) return;

    await this.service
      .removeDashboard({ name: dashboard.name, body: '"' + url + '"' })
      .toAsync()
      .catch(
        async (e: HttpErrorResponse) =>
          await this.moryxSnackbar.showError(e.error.title)
      );

    this.dashboards.update(_=> this.dashboards().filter((d) => d.url !== url));
    if (url === this.selectedUrl()) {
      this.selectedUrl.set(undefined);
      localStorage.removeItem('lastSelectedDashboard');
    }
  }

  async onEdit(url: string) {
    const dashboard = this.dashboards()?.find((d) => d.url == url);
    if (!dashboard || !dashboard.name) return;

    const dialogRef = this.dialog.open(DialogEditDashboardComponent, {
      data: dashboard,
    });

    const changedDashboard = await dialogRef.afterClosed().toAsync();
    if (!changedDashboard || !changedDashboard.url) return;

    const changedDashboardInformation = <ChangedDashboardInformation>{
      originalUrl: dashboard.url,
      changedDashboard: changedDashboard,
    };

    await this.service
      .editDashboard({
        name: dashboard.name,
        body: changedDashboardInformation,
      })
      .toAsync()
      .catch(
        async (e: HttpErrorResponse) =>
          await this.moryxSnackbar.showError(e.error.title)
      );
    dashboard.name = changedDashboard.name;
    dashboard.url = changedDashboard.url;
    if (url === this.selectedUrl() && dashboard.url !== this.selectedUrl()) {
      this.selectedUrl.set(changedDashboard.url);
      localStorage.setItem('lastSelectedDashboard', changedDashboard.url);
    }
  }
}
