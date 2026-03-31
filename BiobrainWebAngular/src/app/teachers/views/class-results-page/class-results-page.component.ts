import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { filter } from 'rxjs/operators';

import { ActiveCourseService } from '../../../core/services/active-course.service';
import { ActiveSchoolClassService } from '../../../core/services/active-school-class.service';
import { hasValue } from '../../../share/helpers/has-value';

@Component({
  selector: 'app-class-results-page',
  templateUrl: './class-results-page.component.html',
  styleUrls: ['./class-results-page.component.scss'],
})
export class ClassResultsPageComponent {
  readonly schoolClassIdChanges$: Observable<string>;
  readonly courseIdChanges$: Observable<string>;

  constructor(
    private readonly _activeSchoolClassService: ActiveSchoolClassService,
    private readonly _activeCourseService: ActiveCourseService,
  ) {
    this.schoolClassIdChanges$ = this._activeSchoolClassService.schoolClassIdChanges$.pipe(filter(hasValue));
    this.courseIdChanges$ = this._activeCourseService.courseIdChanges$.pipe(filter(hasValue));
  }
}
