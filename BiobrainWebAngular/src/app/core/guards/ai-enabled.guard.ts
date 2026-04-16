import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { RoutingService } from '../../auth/services/routing.service';
import { ActiveSchoolService } from '../services/active-school.service';

@Injectable({
  providedIn: 'root',
})
export class AiEnabledGuard implements CanActivate {
  constructor(
    private readonly _activeSchoolService: ActiveSchoolService,
    private readonly _routingService: RoutingService,
  ) {
  }

  canActivate(
    _route: ActivatedRouteSnapshot,
    _state: RouterStateSnapshot,
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    return this._activeSchoolService.aiDisabledChanges$.pipe(
      switchMap(aiDisabled => aiDisabled ? this._routingService.homePage() : Promise.resolve(true)),
    );
  }
}
