import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { CurrentUserService } from 'src/app/auth/services/current-user.service';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { StringsService } from 'src/app/share/strings.service';

import { hasValue } from '../../share/helpers/has-value';
import { AppEventProvider } from '../app/app-event-provider.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {

  constructor(
    private readonly _routingService: RoutingService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _appEvents: AppEventProvider,
    private readonly _strings: StringsService,
  ) {
  }

  async canActivate(route: ActivatedRouteSnapshot, routerStateSnapshot: RouterStateSnapshot): Promise<boolean | UrlTree> {
    const user = await this._currentUserService.user;
    if (!hasValue(user)) {
      await this._routingService.navigateToLoginPage(routerStateSnapshot.url);
      return false;
    }

    const allowedForRoles = route.data['roles'] as string[] || [];
    if (allowedForRoles.length === 0) {
      return true;
    }

    if (user.roles.some(role => allowedForRoles.includes(role))) {
      return true;
    }

    await this._routingService.navigateToHome();
    this._appEvents.errorEmit(this._strings.permissionDeniedError);
    return false;
  }
}
