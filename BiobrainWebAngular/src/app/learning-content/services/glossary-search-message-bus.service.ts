import { Injectable } from '@angular/core';
import { interval, Observable, ReplaySubject } from 'rxjs';
import { debounce, startWith } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class GlossarySearchMessageBus {
  readonly search$: Observable<SearchData>;
  private readonly _searchSubject$ = new ReplaySubject<SearchData>(1);

  constructor() {
    const debounceIntervalMs = 200;
    this.search$ = this._searchSubject$.asObservable().pipe(
      startWith({
        algorithm: SearchAlgorithms.Unknown,
        searchText: undefined
      }),
      debounce(() => interval(debounceIntervalMs))
    );
  }

  next(searchData: SearchData): void {
    this._searchSubject$.next(searchData);
  }
}

export interface SearchData {
  algorithm: SearchAlgorithms;
  searchText: string | undefined | null;
}

export enum SearchAlgorithms {
  Unknown,
  HeaderContains,
  StartFromLetter
}
