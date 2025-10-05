import { Component, OnInit, Inject, signal } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { DashboardInformation } from 'src/app/api/models';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';


@Component({
    selector: 'app-dialog-edit-dashboard',
    templateUrl: './dialog-edit-dashboard.component.html',
    styleUrls: ['./dialog-edit-dashboard.component.scss'],
    standalone: true,
    imports:[
      FormsModule,
      ReactiveFormsModule,
      MatInputModule,
      TranslateModule,
      MatDialogModule,
      MatButtonModule
    ]
})
export class DialogEditDashboardComponent implements OnInit {
  TranslationConstants = TranslationConstants;
  result = signal<DashboardInformation>(<DashboardInformation>{});

  constructor(
    public dialogRef: MatDialogRef<DialogEditDashboardComponent>,
    @Inject(MAT_DIALOG_DATA) public data:DashboardInformation,
    public translate: TranslateService
  ) { 
    this.result.update(e => {
      e.name = data.name;
      e.url = data.url;
      return e;
    })
  }

  ngOnInit(): void {
  }

  onClose() {
    this.dialogRef.close();
  }

}
