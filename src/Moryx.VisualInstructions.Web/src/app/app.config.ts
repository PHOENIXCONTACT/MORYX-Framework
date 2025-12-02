import { HttpClient, provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { ApplicationConfig, importProvidersFrom, Injectable } from "@angular/core";
import { TranslateHttpLoader } from "@ngx-translate/http-loader";
import { environment } from "src/environments/environment";
import { ApiModule } from "./api/api.module";
import { BrowserModule, HAMMER_GESTURE_CONFIG, HammerGestureConfig, HammerModule } from "@angular/platform-browser";
import { MatButtonModule } from "@angular/material/button";
import { MatCardModule } from "@angular/material/card";
import { MatDialogModule } from "@angular/material/dialog";
import { MatDividerModule } from "@angular/material/divider";
import { MatIconModule } from "@angular/material/icon";
import { MatListModule } from "@angular/material/list";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { provideAnimations } from "@angular/platform-browser/animations";
import { ApiInterceptor, API_INTERCEPTOR_PROVIDER, MoryxSnackbarService } from "@moryx/ngx-web-framework";
import { TranslateModule, TranslateLoader } from "@ngx-translate/core";
import { NgxDocViewerModule } from "ngx-doc-viewer";
import { provideRouter, provideRoutes } from "@angular/router";
import { routes } from "./app.routes";
import { MarkdownModule } from "ngx-markdown";

function httpTranslateLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(
    http,
    environment.assets + "assets/languages/"
  );
}

@Injectable()
export class AppHammerConfig extends HammerGestureConfig {
  override overrides = <any>{
    swipe: { direction: Hammer.DIRECTION_HORIZONTAL },
  };
}


export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    importProvidersFrom(
      ApiModule.forRoot({ rootUrl: environment.rootUrl }),
      BrowserModule,
      MatButtonModule,
      MatCardModule,
      MatDialogModule,
      MatDividerModule,
      MatIconModule,
      MatListModule,
      MatProgressSpinnerModule,
      MatSnackBarModule,
      NgxDocViewerModule,
      MarkdownModule.forRoot(),
      TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useFactory: httpTranslateLoaderFactory,
          deps: [HttpClient],
        },
      }),
      HammerModule
    ),
    { provide: HAMMER_GESTURE_CONFIG, useClass: AppHammerConfig },
    ApiInterceptor,
    API_INTERCEPTOR_PROVIDER,
    MoryxSnackbarService,
    provideHttpClient(withInterceptorsFromDi()),
    provideAnimations(),
  ],
};
