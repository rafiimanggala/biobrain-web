import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';


@Injectable({
  providedIn: 'root',
})
export class LoaderService {
  loaderState$: Observable<LoaderState>;
  private readonly _loaderSubject: Subject<LoaderState>;
  private _refCounter = 0;

  constructor() {
    this._loaderSubject = new Subject<LoaderState>();
    this.loaderState$ = this._loaderSubject.asObservable();
  }

  show(): void {
    this._refCounter += 1;
    this._loaderSubject.next({ isLoading: true });
  }

  hide(): void {
    this._refCounter -= 1;
    if (this._refCounter === 0) {
      this._loaderSubject.next({ isLoading: false });
    }
  }

  hideIfVisible(): void {
    if (this._refCounter > 0) this.hide();
  }
}

export interface LoaderState {
  isLoading: boolean;
}
