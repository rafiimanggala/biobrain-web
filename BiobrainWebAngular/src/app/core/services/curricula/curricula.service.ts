import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

import { Api } from '../../../api/api.service';
import { GetCurriculaQuery } from '../../../api/curricula/get-curricula.query';
import { toDictionary } from '../../../share/helpers/observable-operators';

import { Curriculum } from './curriculum';

@Injectable({
  providedIn: 'root',
})
export class CurriculaService {
  public readonly items$: Observable<Curriculum[]>;
  public readonly index$: Observable<Map<number, Curriculum>>;

  constructor(private readonly _api: Api) {
    this.items$ = this._api.send(new GetCurriculaQuery()).pipe(
      map(items => items.map(_ => new Curriculum(_.curriculumCode, _.name))),
      shareReplay(1),
    );

    this.index$ = this.items$.pipe(toDictionary(_ => _.curriculumCode), shareReplay(1));
  }
}
