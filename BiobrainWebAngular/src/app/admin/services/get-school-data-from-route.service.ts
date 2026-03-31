import {
  filter,
  map,
  pluck,
  switchMap
} from 'rxjs/operators';

import { ActivatedRoute } from '@angular/router';
import { GetSchoolByIdQuery } from 'src/app/api/schools/get-school-by-id.query';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Api } from '../../api/api.service';

@Injectable()
export class GetSchoolDataFromRouteService {
  constructor(
    private readonly _route: ActivatedRoute,
    private readonly _api: Api
  ) {}

  getSchoolName(): Observable<string> {
    return this.getSchoolId().pipe(
      map(schoolId => new GetSchoolByIdQuery(schoolId)),
      switchMap(query => this._api.send(query)),
      map(school => school.name)
    );
  }

  getSchoolId(): Observable<string> {
    return this._route.params.pipe(
      pluck('schoolId'),
      filter(schoolId => Boolean(schoolId)),
      map(schoolId => schoolId as string)
    );
  }

  getSnapshotSchoolId(): string {
    const schoolId = this._route.snapshot.paramMap.get('schoolId');
    if (!schoolId) {
      throw new Error('Unable to identify SchoolId from route');
    }

    return schoolId;
  }
}
