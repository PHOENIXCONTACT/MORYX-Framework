import {Component, computed, effect, inject, input, ViewEncapsulation} from '@angular/core';
import {WebModuleItem} from './models/web-module-item';
import {FullLayout} from './full-layout/full-layout';
import {LauncherStateService} from './services/launcher-state.service';
import {ExternalModuleItem} from './models/external-module-item';
import {CultureModel} from './models/culture-model';
import {OperatorLayout} from './operator-layout/operator-layout';
import {FullscreenLayout} from './fullscreen-layout/fullscreen-layout';

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

  private launcherStateService = inject(LauncherStateService);

  layout = computed(() => {
    const state = this.launcherStateService.state();
    if (state.fullscreen) return 'fullscreen';
    if (state.operatorMode) return 'operator';
    return 'full';
  });

  constructor() {
    effect(() => {
      console.log('Module items changed', [...this.webModuleItems(), ...this.externalModuleItems()]);
      console.log('Supported cultures changed', this.supportedCultures());
      console.log('Layout changed', this.layout());
    });
  }
}
