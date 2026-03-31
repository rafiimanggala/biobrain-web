import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { shareReplay, switchMap } from 'rxjs/operators';

import { CurrentUserService } from '../../auth/services/current-user.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { hasValue } from '../../share/helpers/has-value';

import { Course } from './courses/course';
import { CoursesService } from './courses/courses.service';

@Injectable({
  providedIn: 'root',
})
export class ActiveCourseService {
  public readonly courseIdChanges$: BehaviorSubject<string | null>;
  public readonly courseChanges$: Observable<Course | null | undefined>;
  private readonly _activeCourseIdKey = 'activeCourseId';

  constructor(coursesService: CoursesService, currentUserService: CurrentUserService) {
    this.courseIdChanges$ = new BehaviorSubject(localStorage.getItem(this._activeCourseIdKey));
    this.courseChanges$ = this.courseIdChanges$.pipe(
      switchMap(courseId => courseId ? coursesService.findById(courseId) : of(null)),
      shareReplay(1),
    );

    currentUserService.userChanges$.subscribe(user => {
      if (!hasValue(user)) {
        this._cleanActiveCourseId();
      }
    });
  }

  public get courseId(): Promise<string | null> {
    return firstValueFrom(this.courseIdChanges$);
  }

  public getActiveCourseIdFromStorage(): string | null {
    return localStorage.getItem(this._activeCourseIdKey);
  }

  public setActiveCourseId(courseId: string): void {
    localStorage.setItem(this._activeCourseIdKey, courseId);
    this.courseIdChanges$.next(courseId);
  }

  private _cleanActiveCourseId(): void {
    localStorage.removeItem(this._activeCourseIdKey);
    this.courseIdChanges$.next(null);
  }
}
