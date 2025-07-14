import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { DashboardInformation } from 'src/app/api/models'
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';

@Component({
    selector: 'app-dialog-add-dashboard',
    templateUrl: './dialog-add-dashboard.component.html',
    styleUrls: ['./dialog-add-dashboard.component.scss'],
    standalone: true,
    imports: [
      CommonModule,
      TranslateModule,
      FormsModule,
      MatFormFieldModule,
      MatDialogModule,
      MatButtonModule,
      MatFormFieldModule,
      MatInputModule
    ]
})
export class DialogAddDashboardComponent implements OnInit {
  TranslationConstants = TranslationConstants;
  result = signal<DashboardInformation>(<DashboardInformation>{});

  constructor(
    public dialogRef: MatDialogRef<DialogAddDashboardComponent>,
    public tranlate: TranslateService
  ) { }

  ngOnInit(): void {
  }

  onClose() {
    this.dialogRef.close();
  }

}
