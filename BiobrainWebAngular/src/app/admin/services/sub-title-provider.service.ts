import { Observable, ReplaySubject } from 'rxjs';

import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SubTitleProviderService {
  readonly subTitleSubject: ReplaySubject<string>;
  readonly subTitle$: Observable<string>;

  constructor() {
    const bufferSize = 1;
    this.subTitleSubject = new ReplaySubject<string>(bufferSize);
    this.subTitle$ = this.subTitleSubject.asObservable();
  }
}
