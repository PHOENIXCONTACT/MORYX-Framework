import {Component, effect, inject, input, ViewEncapsulation} from '@angular/core';
import {WebModuleItem} from './models/web-module-item';
import {FullLayout} from './full-layout/full-layout';
import {LauncherStateService} from './services/launcher-state.service';
import {ExternalModuleItem} from './models/external-module-item';
import {CultureModel} from './models/culture-model';
import {OperatorLayout} from './operator-layout/operator-layout';
import {FullscreenLayout} from './fullscreen-layout/fullscreen-layout';
import {ModuleService} from './services/module.service';
import {CultureService} from './services/culture.service';
import {AuthService} from './services/auth.service';

@Component({
  selector: 'app-root',
  imports: [FullLayout, OperatorLayout, FullscreenLayout],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  encapsulation: ViewEncapsulation.ShadowDom,
})
export class App {

  // Web component inputs
  webModuleItems = input.required<WebModuleItem[]>();
  externalModuleItems = input.required<ExternalModuleItem[]>();
  supportedCultures = input.required<CultureModel[]>();
  authBaseAddress = input<string>();

  private launcherStateService = inject(LauncherStateService);
  private moduleService = inject(ModuleService);
  private cultureService = inject(CultureService);
  private authService = inject(AuthService);

  layout = this.launcherStateService.layout;

  constructor() {
    effect(() => {
      this.moduleService.modules.set([...this.webModuleItems(), ...this.externalModuleItems()]);
      this.cultureService.supportedCultures.set(this.supportedCultures());

      const authBaseAddress = this.authBaseAddress();
      this.authService.authBaseAddress = authBaseAddress;
      this.authService.authConfigured.set(!!authBaseAddress && authBaseAddress.length > 0);
    });
  }
}
