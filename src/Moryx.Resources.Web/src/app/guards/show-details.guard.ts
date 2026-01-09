import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { PermissionService } from '@moryx/ngx-web-framework';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Permissions } from '../extensions/permissions.extensions';

@Injectable({
  providedIn: 'root',
})
export class ShowDetailsGuard implements CanActivate {
  Permissions = Permissions;
  constructor(private permissionService: PermissionService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (environment.ignoreIam) {
      return true;
    }

    if (window.configs && !window.configs.identityUrl) {
      return true;
    }

    return this.permissionService
      .getPermissions()
      .then(permissions => permissions.any(p => p === Permissions.CAN_VIEW_DETAILS));
  }
}
