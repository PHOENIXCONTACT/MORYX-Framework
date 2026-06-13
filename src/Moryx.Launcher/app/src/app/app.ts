import {Component, effect, input, ViewEncapsulation} from '@angular/core';
import {CultureModel, ExternalModuleItem, WebModuleItem} from './web-module-item';
import {FullLayout} from './full-layout/full-layout';

@Component({
  selector: 'app-root',
  imports: [FullLayout],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  encapsulation: ViewEncapsulation.ShadowDom
})
export class App {

  // Web component inputs
  webModuleItems = input.required<WebModuleItem[]>();
  externalModuleItems = input.required<ExternalModuleItem[]>();
  supportedCultures = input.required<CultureModel[]>();

  constructor() {
    effect(() => {
      console.log('Module items changed', [...this.webModuleItems(), ...this.externalModuleItems()]);
      console.log('Supported cultures changed', this.supportedCultures());
    });
  }
}
