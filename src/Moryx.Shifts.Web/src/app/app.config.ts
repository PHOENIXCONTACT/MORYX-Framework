import { DragDropModule } from "@angular/cdk/drag-drop";
import { HttpClient, provideHttpClient, withInterceptorsFromDi } from "@angular/common/http";
import { ApplicationConfig, importProvidersFrom } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatButtonToggleModule } from "@angular/material/button-toggle";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { provideNativeDateAdapter } from "@angular/material/core";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatDialogModule } from "@angular/material/dialog";
import { MatDividerModule } from "@angular/material/divider";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatListModule } from "@angular/material/list";
import { MatMenuModule } from "@angular/material/menu";
import { MatSelectModule } from "@angular/material/select";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatTabsModule } from "@angular/material/tabs";
import { MatTooltipModule } from "@angular/material/tooltip";
import { BrowserModule } from "@angular/platform-browser";
import { provideAnimationsAsync } from "@angular/platform-browser/animations/async";
import { environment } from "src/environments/environment";
import { ApiModule } from "./api/api.module";
import { AppStoreService } from "./services/app-store.service";
import { AssignmentService } from "./services/assignment.service";
import { ShiftService } from "./services/shift.service";
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import {
    TranslateService,
    TranslateModule,
    TranslateLoader,
  } from '@ngx-translate/core';
import { provideRouter } from "@angular/router";
import { routes } from "./app.routes";

  export function httpTranslateLoaderFactory(http: HttpClient) {
    return new TranslateHttpLoader(http, environment.assets + 'assets/languages/');
  }
  
export const appConfig : ApplicationConfig = {
    providers : [
        provideRouter(routes),
        provideHttpClient(withInterceptorsFromDi()),
        importProvidersFrom(
            BrowserModule,
            MatButtonModule,
            MatIconModule,
            MatMenuModule,
            MatInputModule,
            MatCheckboxModule,
            MatDividerModule,
            MatSidenavModule,
            MatTabsModule,
            MatListModule,
            DragDropModule,
            MatDialogModule,
            FormsModule,
            ReactiveFormsModule,
            MatSelectModule,
            MatTooltipModule,
            MatButtonToggleModule,
            MatDatepickerModule,
            MatExpansionModule,
            ApiModule.forRoot({ rootUrl: environment.rootUrl }),
            TranslateModule.forRoot({
              loader: {
                provide: TranslateLoader,
                useFactory: httpTranslateLoaderFactory,
                deps: [HttpClient],
              },
            })
          ),
          provideAnimationsAsync(),
          provideNativeDateAdapter(),
          ShiftService,
          AppStoreService,
          AssignmentService,
          TranslateService
    ]
}