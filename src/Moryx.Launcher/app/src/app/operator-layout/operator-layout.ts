import {Component, computed, input} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {WebModuleItem} from '../models/web-module-item';
import {ExternalModuleItem} from '../models/external-module-item';
import {CultureModel} from '../models/culture-model';
import {ModuleCategory} from '../models/module-category';
import {HorizontalNav} from '../horizontal-nav/horizontal-nav';

@Component({
  selector: 'app-operator-layout',
  imports: [RouterOutlet, HorizontalNav],
  templateUrl: './operator-layout.html',
  styleUrl: './operator-layout.scss',
})
export class OperatorLayout {

  webModuleItems = input.required<WebModuleItem[]>();
  externalModuleItems = input.required<ExternalModuleItem[]>();
  supportedCultures = input.required<CultureModel[]>();

  userModules = computed(() => {
    return [...this.webModuleItems(), ...this.externalModuleItems()]
      .filter(m => m.category === ModuleCategory.User)
      .sort((a, b) => a.sortIndex - b.sortIndex);
  });
}
