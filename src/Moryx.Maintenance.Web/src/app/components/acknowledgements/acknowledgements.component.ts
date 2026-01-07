import { Component, input, signal } from '@angular/core';
import { Acknowledgement } from '../../models/acknowledgement';
import { CommonModule } from '@angular/common';
import { EmptyStateComponent } from '@moryx/ngx-web-framework';
import { TranslateModule } from '@ngx-translate/core';
import { TranslationConstants } from '../../extensions/translation-constants.extensions';

@Component({
  selector: 'app-acknowledgements',
  imports: [
    CommonModule,
    EmptyStateComponent,
    TranslateModule
  ],
  templateUrl: './acknowledgements.component.html',
  styleUrl: './acknowledgements.component.scss'
})
export class AcknowledgementsComponent {
  acknowledgements = input.required<Acknowledgement[]>();

  TranslationConstants = TranslationConstants;
}
