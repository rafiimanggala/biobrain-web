import { Injectable } from '@angular/core';
import { forkJoin, Observable } from 'rxjs';
import { first, map } from 'rxjs/operators';
import { GetClassStudentsListQuery } from 'src/app/api/students/get-class-students-list.query';

import { Api } from '../../../api/api.service';
import { GetSchoolStudentsListQuery } from '../../../api/students/get-school-students-list.query';
import { ObservableCache } from '../../../share/helpers/observable-cache';
import { toDictionary } from '../../../share/helpers/observable-operators';
import { toLookupFromList } from '../../../share/helpers/to-lookup';
import { toNonNullable } from '../../../share/helpers/to-non-nullable';

import { Student } from './student';

@Injectable({
  providedIn: 'root',
})
export class StudentsService {
  private readonly _studentsCache = new ObservableCache<Student[]>();
  private readonly _studentsByClassIndexCache = new ObservableCache<Map<string | null | undefined, Student[]>>();
  private readonly _studentByIdIndexCache = new ObservableCache<Map<string, Student>>();

  constructor(private readonly _api: Api) {
  }

  public getStudents(schoolId: string): Observable<Student[]> {
    return this._studentsCache.get(schoolId, () => this._getStudentsForSchool(schoolId));
  }

  public async refreshAllCaches(){
    await this._studentsCache.reload();
    await this._studentsByClassIndexCache.reload();
    await this._studentByIdIndexCache.reload();
  }

  public getById(schoolId: string, studentId: string): Observable<Student> {
    return this.getStudentsByIdIndex(schoolId).pipe(map(_ => _.get(studentId)), map(toNonNullable));
  }

  public getByIds(schoolId: string, studentIds: string[]): Observable<Student[]> {
    return forkJoin(studentIds.map(studentId => this.getById(schoolId, studentId).pipe(first())));
  }

  public getForClassFromCache(schoolId: string, schoolClassId: string): Observable<Student[]> {
    return this.getStudentsByClassIndex(schoolId).pipe(map(_ => _.get(schoolClassId) ?? []));
  }

  public getForClassFromApi(schoolId: string, schoolClassId: string): Observable<Student[]> {
    const query = new GetClassStudentsListQuery(schoolId, schoolClassId);
    return this._api.send(query).pipe(
      map(student =>
        student.map(_ => new Student(
          _.studentId,
          _.firstName,
          _.lastName,
          _.email,
          _.schoolId,
          _.schoolClassIds,
        )),
      ),
    );
  }

  public getStudentsByIdIndex(schoolId: string): Observable<Map<string, Student>> {
    return this._studentByIdIndexCache.get(schoolId, () =>
      this.getStudents(schoolId).pipe(toDictionary(_ => _.studentId)),
    );
  }

  public getStudentsByClassIndex(schoolId: string): Observable<Map<string | null | undefined, Student[]>> {
    return this._studentsByClassIndexCache.get(schoolId, () =>
      this.getStudents(schoolId).pipe(toLookupFromList(_ => _.schoolClassIds)),
    );
  }

  private _getStudentsForSchool(schoolId: string): Observable<Student[]> {
    return this._api.send(new GetSchoolStudentsListQuery(schoolId)).pipe(
      map(student =>
        student.map(_ => new Student(
          _.studentId,
          _.firstName,
          _.lastName,
          _.email,
          _.schoolId,
          _.schoolClassIds,
        )),
      ),
    );
  }
}
