import { Component, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import Order from 'src/app/models/order';
import { OrderStoreService } from 'src/app/services/order-store.service';
import { NgFor, AsyncPipe, CommonModule } from '@angular/common';

@Component({
    selector: 'app-orders-container',
    templateUrl: './orders-container.component.html',
    styleUrls: ['./orders-container.component.scss'],
    imports: [CommonModule],
    standalone: true
})
export class OrdersContainerComponent {
  TranslationConstants = TranslationConstants;
  orderStoreService= inject(OrderStoreService);
  translate = inject(TranslateService);
}
