import {Component, computed, input} from '@angular/core';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatMenuModule} from '@angular/material/menu';
import {MatDividerModule} from '@angular/material/divider';
import {CultureModel, ModuleItem} from '../web-module-item';
import {localLanguage} from '../utils';

@Component({
  selector: 'app-more-menu',
  imports: [MatButtonModule, MatIconModule, MatMenuModule, MatDividerModule],
  templateUrl: './more-menu.html',
  styleUrl: './more-menu.scss'
})
export class MoreMenu {
  supportedCultures = input.required<CultureModel[]>();
  modules = input.required<ModuleItem[]>();

  currentCulture = computed(() => localLanguage());

  selectCulture(culture: CultureModel) {
    let cookieDate = new Date;
    cookieDate.setFullYear(cookieDate.getFullYear() + 1);
    const value = encodeURIComponent(`c=${culture.name}|uic=${culture.name}`);
    document.cookie = `.AspNetCore.Culture=${value};path=/;expires=${cookieDate.toUTCString()}`;
    window.location.reload();
  }
}
