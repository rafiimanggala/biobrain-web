import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

import { Api } from '../../../api/api.service';
import { GetSubjectsQuery } from '../../../api/subjects/get-subjects.query';
import { toDictionary } from '../../../share/helpers/observable-operators';

import { Subject } from './subject';

@Injectable({
  providedIn: 'root',
})
export class SubjectsService {
  public readonly biology = 1;
  public readonly chemistry = 2;
  public readonly physics = 3;
  public readonly biology10 = 5;
  public readonly chemistry10 = 6;
  public readonly forensics = 7;
  public readonly physics10 = 8;
  public readonly marine10 = 9;
  public readonly psychology = 10;
  public readonly science = 11;
  public readonly live = 13;
  public readonly sciencePen = 14;
  public readonly biology10Us = 15;
  public readonly earth10Jpc = 16;
  public readonly applied10Jpc = 17;

  public readonly items$: Observable<Subject[]>;
  public readonly index$: Observable<Map<number, Subject>>;

  constructor(private readonly _api: Api) {
    this.items$ = this._api.send(new GetSubjectsQuery()).pipe(
      map(items => items.map(_ => new Subject(_.subjectCode, _.name))),
      shareReplay(1),
    );

    this.index$ = this.items$.pipe(toDictionary(_ => _.subjectCode), shareReplay(1));
  }

  public findById(subjectCode: number): Observable<Subject | undefined> {
    return this.index$.pipe(map(index => index.get(subjectCode)));
  }
}
