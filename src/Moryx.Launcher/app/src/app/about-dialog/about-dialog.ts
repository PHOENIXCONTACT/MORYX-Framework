import {Component, computed, inject} from '@angular/core';
import {toSignal} from '@angular/core/rxjs-interop';
import {timer} from 'rxjs';
import {catchError, switchMap} from 'rxjs/operators';
import {of} from 'rxjs';
import {MatButtonModule} from '@angular/material/button';
import {MatDialogModule} from '@angular/material/dialog';
import {CommonService} from '@api/services/common.service';
import {localLanguage} from '../utils';

@Component({
  selector: 'app-about-dialog',
  imports: [MatDialogModule, MatButtonModule],
  templateUrl: './about-dialog.html',
  styleUrl: './about-dialog.scss',
})
export class AboutDialog {

  private commonService = inject(CommonService);

  private applicationInfo = toSignal(this.commonService.getApplicationInfo()
    .pipe(catchError(() => of(null))));

  private hostInfo = toSignal(this.commonService.getHostInfo()
    .pipe(catchError(() => of(null))));

  private rawServerTime = toSignal(timer(0, 1000)
    .pipe(switchMap(() =>
      this.commonService.getServerTime().pipe(catchError(() => of(null))))
  ));

  serverTime = computed(() => {
    const rawServerTime = this.rawServerTime()?.serverTime;
    if (!rawServerTime) {
      return null;
    }
    return new Intl.DateTimeFormat(localLanguage() || undefined, {
      dateStyle: 'medium',
      timeStyle: 'medium',
    }).format(new Date(rawServerTime));
  });

  appEntries = computed(() => {
    const applicationInfo = this.applicationInfo();
    if (!applicationInfo) {
      return [];
    }
    return [
      {label: 'Title', value: applicationInfo.assemblyTitle},
      {label: 'Product', value: applicationInfo.assemblyProduct},
      {label: 'Description', value: applicationInfo.assemblyDescription},
      {label: 'Version', value: applicationInfo.assemblyVersion},
      {label: 'Informational Version', value: applicationInfo.assemblyInformationalVersion},
      {label: 'Company', value: applicationInfo.assemblyCompanyName},
      {label: 'Configuration', value: applicationInfo.assemblyConfiguration},
      {label: 'Copyright', value: applicationInfo.assemblyCopyright},
      {label: 'Target Framework', value: applicationInfo.targetFramework},
    ].filter(e => e.value);
  });

  hostEntries = computed(() => {
    const hostInfo = this.hostInfo();
    if (!hostInfo) {
      return [];
    }
    return [
      {label: 'Machine Name', value: hostInfo.machineName},
      {label: 'OS', value: hostInfo.osInformation},
      {label: 'Uptime', value: hostInfo.upTime != null ? this.formatUptime(hostInfo.upTime) : null},
    ].filter(e => e.value);
  });

  private formatUptime(milliseconds: number): string {
    const seconds = milliseconds / 1000;
    const d = Math.floor(seconds / 86400);
    const h = Math.floor((seconds % 86400) / 3600);
    const m = Math.floor((seconds % 3600) / 60);
    const parts = [];
    if (d) parts.push(`${d}d`);
    if (h) parts.push(`${h}h`);
    parts.push(`${m}m`);
    return parts.join(' ');
  }
}
