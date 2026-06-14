import {Component, HostListener, inject, input} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {WebModuleItem} from '../models/web-module-item';
import {ExternalModuleItem} from '../models/external-module-item';
import {CultureModel} from '../models/culture-model';
import {LauncherStateService} from '../services/launcher-state.service';

@Component({
  selector: 'app-fullscreen-layout',
  imports: [RouterOutlet],
  templateUrl: './fullscreen-layout.html',
  styleUrl: './fullscreen-layout.scss',
})
export class FullscreenLayout {
  webModuleItems = input.required<WebModuleItem[]>();
  externalModuleItems = input.required<ExternalModuleItem[]>();
  supportedCultures = input.required<CultureModel[]>();

  private launcherStateService = inject(LauncherStateService);

  @HostListener('window:keydown.escape')
  exitFullscreen() {
    this.launcherStateService.updateState({ fullscreen: false, operatorMode: false });
  }
}
