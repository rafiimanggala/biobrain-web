import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { RoutingService } from '../../auth/services/routing.service';
import { hasValue } from '../../share/helpers/has-value';
import { ActiveCourseService } from '../services/active-course.service';

@Injectable({
  providedIn: 'root',
})
export class CourseSelectedGuard implements CanActivate {
  constructor(
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _routingService: RoutingService) {
  }

  canActivate(
    _route: ActivatedRouteSnapshot,
    _state: RouterStateSnapshot,
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    return this._activeCourseService.courseIdChanges$.pipe(
      switchMap(courseId => !hasValue(courseId) ? this._routingService.homePage() : Promise.resolve(true)),
    );
  }
}
