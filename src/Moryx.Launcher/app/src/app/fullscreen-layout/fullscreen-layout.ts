import {Component, HostListener, inject} from '@angular/core';
import {LauncherLayout, LauncherStateService} from '../services/launcher-state.service';

@Component({
  selector: 'app-fullscreen-layout',
  imports: [],
  templateUrl: './fullscreen-layout.html',
  styleUrl: './fullscreen-layout.scss',
})
export class FullscreenLayout {

  private launcherStateService = inject(LauncherStateService);

  @HostListener('window:keydown.escape')
  exitFullscreen() {
    this.launcherStateService.updateLayout(LauncherLayout.Full);
  }
}
