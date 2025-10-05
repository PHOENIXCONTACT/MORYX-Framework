import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { ResourceMethodService } from '../../../services/resource-method.service';
import {
  MatExpansionModule,
} from '@angular/material/expansion';
import { EntryEditorComponent, NavigableEntryEditorComponent } from '@moryx/ngx-web-framework';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-resource-methods',
  templateUrl: './resource-methods.component.html',
  styleUrls: ['./resource-methods.component.scss'],
  imports: [
    CommonModule,
    EntryEditorComponent,
    MatExpansionModule,
    NavigableEntryEditorComponent,
    TranslateModule,
    MatButtonModule,
  ],
  standalone: true,
})
export class ResourceMethodsComponent implements OnInit, OnDestroy {
  TranslationConstants = TranslationConstants;

  constructor(public methodService: ResourceMethodService, public translate: TranslateService) {}

  ngOnInit(): void {}

  ngOnDestroy(): void {
    this.methodService.selectedMethod = undefined;
  }
}
