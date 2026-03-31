import { Injectable } from '@angular/core';
import { Event, NavigationEnd, Router } from '@angular/router';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { distinctUntilChanged, map, switchMap } from 'rxjs/operators';

@Injectable()
export class SidenavService {
  public readonly showNavigation$: Observable<boolean>;
  public readonly isNavigationStatic$: Observable<boolean>;

  private readonly _windowWidth$: BehaviorSubject<number>;
  private readonly _isOpened$ = new BehaviorSubject(false);

  constructor(private readonly _router: Router) {
    this._windowWidth$ = new BehaviorSubject(window.innerWidth);
    const screenSize$ = this._windowWidth$.pipe(map(width => width < 960 ? 'small' : 'big'), distinctUntilChanged());
    this.isNavigationStatic$ = screenSize$.pipe(map(screenSize => screenSize === 'big'));

    this.showNavigation$ = this.isNavigationStatic$.pipe(
      switchMap(isNavigationStatic => isNavigationStatic ? of(true) : this._isOpened$),
    );

    this._router.events.subscribe((event: Event) => {
      if (event instanceof NavigationEnd) this.close();
    });
  }

  public open(): void {
    return this._isOpened$.next(true);
  }

  public close(): void {
    return this._isOpened$.next(false);
  }

  public toggle(): void {
    return this._isOpened$.next(!this._isOpened$.getValue());
  }

  public updateWindowSize(value: number): void {
    this._windowWidth$.next(value);
  }
}

export enum SideNavType {
  Tree = 'tree',
  QuizResult = 'quizResilt',
  ResultsSummary = 'resultsSummary',
  Glossary = 'glossary',
}
