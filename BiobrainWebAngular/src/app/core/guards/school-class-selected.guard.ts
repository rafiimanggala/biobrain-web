import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { RoutingService } from '../../auth/services/routing.service';
import { hasValue } from '../../share/helpers/has-value';
import { ActiveSchoolClassService } from '../services/active-school-class.service';

@Injectable({
  providedIn: 'root',
})
export class SchoolClassSelectedGuard implements CanActivate {
  constructor(
    private readonly _activeSchoolClassService: ActiveSchoolClassService,
    private readonly _routingService: RoutingService) {
  }

  canActivate(
    _route: ActivatedRouteSnapshot,
    _state: RouterStateSnapshot,
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    return this._activeSchoolClassService.schoolClassIdChanges$.pipe(
      switchMap(schoolClassId => !hasValue(schoolClassId) ? this._routingService.homePage() : Promise.resolve(true)),
    );
  }
}
