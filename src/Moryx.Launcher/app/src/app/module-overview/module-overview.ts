import { Component, computed, input } from '@angular/core';
import { ModuleCategory } from '../models/module-category';
import { WebModuleItem } from '../models/web-module-item';

@Component({
  selector: 'app-module-overview',
  imports: [],
  templateUrl: './module-overview.html',
  styleUrl: './module-overview.scss',
})
export class ModuleOverview {
  webModuleItems = input.required<WebModuleItem[]>();
  userModules = computed(() => this.webModuleItems().filter(m => m.category === ModuleCategory.User));
}
