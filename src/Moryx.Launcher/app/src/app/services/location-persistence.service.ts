/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { inject, Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { ModuleItem } from '../models/module-item';
import { ModuleService } from './module.service';

@Injectable({
  providedIn: 'root'
})
/** Saves and restores the last visited sub-route per module using cookies. */
export class LocationPersistenceService {
  private moduleService = inject(ModuleService);
  private cookieService = inject(CookieService);

  constructor() {
    window.addEventListener('beforeunload', () => this.saveLocation());
  }

  resolveHref(module: ModuleItem): string {
    if (module.route === this.moduleService.activeRoute()) {
      return module.route;
    }
    const cookieValue = this.cookieService.get(this.cookieName(module.route));
    return cookieValue || module.route;
  }

  private saveLocation(): void {
    const activeRoute = this.moduleService.activeRoute();
    if (!activeRoute) {
      return;
    }
    const pathname = window.location.pathname;
    if (pathname === activeRoute || pathname === activeRoute + '/') {
      return;
    }
    // Expire after 1 hour
    const expires = new Date(Date.now() + 3600 * 1000);
    this.cookieService.set(this.cookieName(activeRoute), pathname, { expires, path: '/' });
  }

  private cookieName(route: string): string {
    // Strip leading slash to match legacy shell.js cookie names
    // TODO: Replace by `route + '-location'` in next major version
    return route.replace(/^\//, '') + '-location';
  }
}
