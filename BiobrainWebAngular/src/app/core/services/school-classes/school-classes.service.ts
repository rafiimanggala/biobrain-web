import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { Api } from '../../../api/api.service';
import { GetSchoolClassesListQuery } from '../../../api/school-classes/get-school-classes-list.query';
import { ObservableCache } from '../../../share/helpers/observable-cache';
import { toDictionary } from '../../../share/helpers/observable-operators';
import { toNonNullable } from '../../../share/helpers/to-non-nullable';

import { SchoolClassCacheModel } from './school-class';

@Injectable({
  providedIn: 'root',
})
export class SchoolClassesCacheService {
  private readonly _classesCache = new ObservableCache<SchoolClassCacheModel[]>();
  private readonly _classesByIdIndexCache = new ObservableCache<Map<string, SchoolClassCacheModel>>();

  constructor(private readonly _api: Api) {
  }

  public getClasses(schoolId: string): Observable<SchoolClassCacheModel[]> {
    return this._classesCache.get(schoolId, () => this._getClassesForSchool(schoolId));
  }

  public getById(schoolId: string, schoolClassId: string): Observable<SchoolClassCacheModel> {
    return this.getClassesByIdIndex(schoolId).pipe(map(_ => _.get(schoolClassId)), map(toNonNullable));
  }

  public getByIds(schoolId: string, schoolClassIds: string[]): Observable<SchoolClassCacheModel[]> {
    return this.getClasses(schoolId)
      .pipe(
        map(
          items => items.filter(
            item => schoolClassIds.includes(item.schoolClassId)
          )
        )
      );
  }

  public getClassesByIdIndex(schoolId: string): Observable<Map<string, SchoolClassCacheModel>> {
    return this._classesByIdIndexCache.get(schoolId, () =>
      this.getClasses(schoolId).pipe(toDictionary(_ => _.schoolClassId)),
    );
  }

  private _getClassesForSchool(schoolId: string): Observable<SchoolClassCacheModel[]> {
    return this._api.send(new GetSchoolClassesListQuery(schoolId)).pipe(
      map(schoolClasses =>
        schoolClasses.map(_ => new SchoolClassCacheModel(
          _.schoolClassId,
          _.schoolId,
          _.name,
          _.year,
          _.autoJoinClassCode,
        )),
      ),
    );
  }
}

