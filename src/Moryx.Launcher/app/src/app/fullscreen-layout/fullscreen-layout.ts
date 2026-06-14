import {Component, HostListener, inject} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {LauncherStateService} from '../services/launcher-state.service';

@Component({
  selector: 'app-fullscreen-layout',
  imports: [RouterOutlet],
  templateUrl: './fullscreen-layout.html',
  styleUrl: './fullscreen-layout.scss',
})
export class FullscreenLayout {

  private launcherStateService = inject(LauncherStateService);

  @HostListener('window:keydown.escape')
  exitFullscreen() {
    this.launcherStateService.updateState({ fullscreen: false, operatorMode: false });
  }
}
