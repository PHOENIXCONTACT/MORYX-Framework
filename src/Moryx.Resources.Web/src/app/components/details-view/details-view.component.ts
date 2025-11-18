import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { Event, NavigationCancel, NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
import { TranslationConstants } from 'src/app/extensions/translation-constants.extensions';
import { SessionService } from 'src/app/services/session.service';
import { ResourceModel } from '../../api/models';
import { EditResourceService } from '../../services/edit-resource.service';
import { Permissions } from './../../extensions/permissions.extensions';
import { MatTabsModule } from '@angular/material/tabs';
import { DetailsHeaderComponent } from './details-header/details-header.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-details-view',
  templateUrl: './details-view.component.html',
  styleUrls: ['./details-view.component.scss'],
  imports: [RouterOutlet, TranslateModule, MatTabsModule, RouterLink, DetailsHeaderComponent, CommonModule],
  standalone: true,
})
export class DetailsViewComponent implements OnInit, OnDestroy {
  activeLink = signal<number | undefined>(undefined);
  resource = signal<ResourceModel>({});

  TranslationConstants = TranslationConstants;
  Permissions = Permissions;

  private oldResourceId?: number;
  private editServiceSubscription?: Subscription;

  constructor(
    private router: Router,
    private sessionService: SessionService,
    public editService: EditResourceService,
    public translate: TranslateService
  ) {}

  ngOnInit() {
    this.router.events.subscribe(event => this.onRoutingEvent(event));
    this.editServiceSubscription = this.editService.activeResource$.subscribe(resource => this.onNewResource(resource));
  }

  ngOnDestroy(): void {
    this.editServiceSubscription?.unsubscribe();
  }

  private onNewResource(resource: ResourceModel | undefined) {
    if (!resource) return;

    this.resource.update(() => resource);

    if (this.oldResourceId === resource?.id) return;

    // ToDo: move to edit resource service
    const wipResource = this.sessionService.getWipResource();

    const url = this.router.url;
    const regexMethods: RegExp = /(details\/\d*\/methods)/;
    const regexReferences: RegExp = /(details\/\d*\/references)/;
    if (regexMethods.test(url)) {
      this.router.navigate([`details/${resource?.id}/methods`]);
    } else if (regexReferences.test(url)) {
      this.router.navigate([`details/${resource?.id}/references`]);
    } else if (resource?.properties) {
      this.router.navigate([`details/${resource.id}/properties`]);
    }
    this.oldResourceId = resource?.id;

    if (wipResource) {
      this.editService.edit = true;
      this.sessionService.removeWipResource();
    }
  }

  private onRoutingEvent(event: Event) {
    if (event instanceof NavigationEnd || event instanceof NavigationCancel) {
      let url = this.router.url;
      const regexProperty: RegExp = /(details\/\d*\/properties)/;
      const regexReferences: RegExp = /(details\/\d*\/references)/;
      const regexMethods: RegExp = /(details\/\d*\/methods)/;
      if (regexProperty.test(url)) {
        this.activeLink.set(1);
      } else if (regexReferences.test(url)) {
        this.activeLink.set(2);
      } else if (regexMethods.test(url)) {
        this.activeLink.set(3);
      }
    }
  }
}
