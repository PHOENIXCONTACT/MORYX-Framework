import {Component, input} from '@angular/core';
import {MatListModule} from '@angular/material/list';
import {MatIconModule} from '@angular/material/icon';
import {ExternalModuleItem, WebModuleItem} from '../web-module-item';
import {NotificationBadge} from '../notification-badge/notification-badge';

@Component({
  selector: 'app-vertical-nav',
  imports: [MatListModule, MatIconModule, NotificationBadge],
  templateUrl: './vertical-nav.html',
  styleUrl: './vertical-nav.scss',
  host: {
    '[class.collapsed]': 'collapsed()',
  }
})
export class VerticalNav {
  moduleItems = input.required<(WebModuleItem | ExternalModuleItem)[]>();
  collapsed = input(false);

  asWeb(item: WebModuleItem | ExternalModuleItem): WebModuleItem | null {
    return 'eventStream' in item ? item as WebModuleItem : null;
  }
}
