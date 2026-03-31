import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { CurrentUserService } from 'src/app/auth/services/current-user.service';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { StringsService } from 'src/app/share/strings.service';

import { hasValue } from '../../share/helpers/has-value';
import { AppEventProvider } from '../app/app-event-provider.service';
import { StudentCoursesService } from '../services/courses/student-courses.service';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { ActiveCourseService } from '../services/active-course.service';

@Injectable({ providedIn: 'root' })
export class SubscriptionGuard implements CanActivate {

  constructor(
    private readonly _routingService: RoutingService,
    private readonly _currentUserService: CurrentUserService,    
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _studentCoursesStore: StudentCoursesService,
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

    if(user.isStudent()){
      var courses = await firstValueFrom(this._studentCoursesStore.getStudentCourses(user.userId));
      var selectedCourse = await this._activeCourseService.courseId;
      if(selectedCourse && !courses.some(_ => _.courses.some(c => c.courseId == selectedCourse))){
        this._appEvents.errorEmit(this._strings.courseNotAvailable);
        await this._routingService.navigateToMyCourses();
        return false;
      }
    }
    //ToDo: Подписка или класс не в архивной школе
    // if(user.isStandalone() && !user.isSubscriptionValid()){
    //   this._appEvents.errorEmit(this._strings.noValidSubscription);
    //   await this._routingService.navigateToSubscriptionPage();
    //   return false;
    // }

    return true;
  }
}
