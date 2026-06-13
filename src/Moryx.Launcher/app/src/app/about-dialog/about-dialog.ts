import {Component, computed, inject} from '@angular/core';
import {toSignal} from '@angular/core/rxjs-interop';
import {MatButtonModule} from '@angular/material/button';
import {MatDialogModule} from '@angular/material/dialog';
import {CommonService} from '../api/services/common.service';

@Component({
  selector: 'app-about-dialog',
  imports: [MatDialogModule, MatButtonModule],
  templateUrl: './about-dialog.html',
  styleUrl: './about-dialog.scss',
})
export class AboutDialog {

  private commonService = inject(CommonService);

  private applicationInfo = toSignal(this.commonService.getApplicationInfo());

  entries = computed(() => {
    const applicationInfo = this.applicationInfo();
    if (!applicationInfo) {
      return [];
    }

    return [
      {label: 'Product', value: applicationInfo.assemblyProduct},
      {label: 'Title', value: applicationInfo.assemblyTitle},
      {label: 'Version', value: applicationInfo.assemblyVersion},
      {label: 'Informational Version', value: applicationInfo.assemblyInformationalVersion},
      {label: 'Company', value: applicationInfo.assemblyCompanyName},
      {label: 'Description', value: applicationInfo.assemblyDescription},
      {label: 'Copyright', value: applicationInfo.assemblyCopyright},
      {label: 'Configuration', value: applicationInfo.assemblyConfiguration},
      {label: 'Target Framework', value: applicationInfo.targetFramework},
    ].filter(e => e.value);
  });
}
