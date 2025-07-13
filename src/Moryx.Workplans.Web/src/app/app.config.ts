import { DragDropModule } from '@angular/cdk/drag-drop';
import { HttpClient, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule } from '@angular/material/dialog';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BrowserModule, HammerModule } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';
import { ApiInterceptor, API_INTERCEPTOR_PROVIDER } from '@moryx/ngx-web-framework';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { environment } from 'src/environments/environment';
import { ApiModule } from './api/api.module';
import { BrowserStorageService } from './services/browser-storage.service';
import { EditorStateService } from './services/editor-state.service';
import { SessionsService } from './services/sessions.service';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

export function httpTranslateLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, environment.assets + 'assets/languages/');
}
export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    importProvidersFrom(
      BrowserModule,
      ApiModule.forRoot({ rootUrl: environment.rootUrl }),
      DragDropModule,
      HammerModule,
      MatListModule,
      MatButtonModule,
      MatTabsModule,
      MatCardModule,
      MatIconModule,
      MatExpansionModule,
      MatDialogModule,
      MatTooltipModule,
      MatSnackBarModule,
      MatToolbarModule,
      MatSidenavModule,
      FormsModule,
      MatInputModule,
      MatProgressSpinnerModule,
      MatFormFieldModule,
      MatSelectModule,
      MatMenuModule,
      MatTableModule,
      TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: httpTranslateLoaderFactory,
          deps: [HttpClient],
        },
      })
    ),
    BrowserStorageService,
    SessionsService,
    EditorStateService,
    ApiInterceptor,
    API_INTERCEPTOR_PROVIDER,
    provideHttpClient(withInterceptorsFromDi()),
    provideAnimations(),
  ],
};
