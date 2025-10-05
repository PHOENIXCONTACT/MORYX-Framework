import { Component, OnInit, Inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';

@Component({
    selector: 'app-dialog-remove-dashboards',
    templateUrl: './dialog-remove-dashboards.component.html',
    styleUrls: ['./dialog-remove-dashboards.component.scss'],
    standalone: true,
    imports:[
      MatDialogModule,
      TranslateModule,
      MatButtonModule
    ]
})
export class DialogRemoveDashboardsComponent implements OnInit {
  dashboardName = signal("");
  TranslationConstants = TranslationConstants;

  constructor(
    public dialogRef: MatDialogRef<DialogRemoveDashboardsComponent>,
    @Inject(MAT_DIALOG_DATA) public data:string,
    public translate: TranslateService
  ) { 
    this.dashboardName.update(_=> data);
  }

  ngOnInit(): void {
  }

  onClose() {
    this.dialogRef.close();
  }

}
