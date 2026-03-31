import { DOCUMENT } from '@angular/common';
import { Inject, Injectable, OnDestroy } from '@angular/core';
import { Observable, of, Subscription } from 'rxjs';
import { catchError, distinctUntilChanged, map, shareReplay, tap } from 'rxjs/operators';

import { assertHasValue } from '../../share/helpers/assert-has-value';
import { Colors } from '../../share/values/colors';
import { ActiveCourseService } from '../services/active-course.service';
import { SubjectsService } from '../services/subjects/subjects.service';

import { AppEventProvider } from './app-event-provider.service';

@Injectable({
  providedIn: 'root',
})
export class ThemeService implements OnDestroy {
  readonly colors$: Observable<SubjectColors>;
  colors: SubjectColors | undefined;
  private readonly _subscriptions: Subscription[] = [];
  private _appliedTheme: Themes;


  constructor(
    @Inject(DOCUMENT) private readonly _document: Document,
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _subjectService: SubjectsService,
    private readonly _appEvents: AppEventProvider,
  ) {
    this._subscriptions.push(
      _activeCourseService.courseChanges$.pipe(
        map(x => x?.subjectCode),
        map(x => this._resolveClassName(x)),
        distinctUntilChanged(),
        catchError(e => {
          this._appEvents.errorEmit(e);
          return of(undefined);
        }),
      ).subscribe(x => this._setTheme(x)),
    );

    this.colors$ = _activeCourseService.courseChanges$.pipe(
      map(x => x?.subjectCode),
      map(x => this._resolveColors(x)),
      distinctUntilChanged(),
      shareReplay(1),
      tap(_ => this.colors = _),
      catchError(e => {
        this._appEvents.errorEmit(e);
        return of({
          primary: Colors.primary,
          accent: Colors.accent
        });
      }),
    );
  }

  ngOnDestroy(): void {
    this._setTheme(undefined);
    this._subscriptions.forEach(s => s.unsubscribe());
  }

  private _setTheme(theme: Themes): void {
    if (this._appliedTheme) {
      this._document.body.classList.remove(this._appliedTheme);
    }

    if (theme) {
      this._document.body.classList.add(theme);
    }

    this._appliedTheme = theme;
  }

  private _resolveClassName(subjectCode: number | undefined): Themes {
    switch (subjectCode) {
      case undefined:
        return undefined;

      case this._subjectService.biology:
      case this._subjectService.biology10:
      case this._subjectService.biology10Us:
      case this._subjectService.science:
      case this._subjectService.sciencePen:
      case this._subjectService.live:
        return undefined;

      case this._subjectService.chemistry:
      case this._subjectService.chemistry10:
        return 'chemistry-theme';

      case this._subjectService.physics:
      case this._subjectService.physics10:
        return 'physics-theme';

      case this._subjectService.forensics:
      case this._subjectService.applied10Jpc:
        return 'forensics-theme';

      case this._subjectService.psychology:
        return 'psychology-theme';

      case this._subjectService.marine10:
      case this._subjectService.earth10Jpc:
        return 'marine-theme';

      default:
        throw new Error(`Unknown subjectCode = ${subjectCode}`);
    }
  }

  public GetSubjectColors(subjectCode: number) {
    return this._resolveColors(subjectCode);
  }

  private _resolveColors(subjectCode: number | undefined): SubjectColors {
    if (subjectCode === undefined) {
      return {
        primary: Colors.primary,
        accent: Colors.accent
      };
    }

    return {
      primary: this._resolvePrimaryColor(subjectCode),
      accent: this._resolveAccentColor(subjectCode)
    };
  }

  private _resolvePrimaryColor(subjectCode: number | undefined): string {
    assertHasValue(subjectCode);

    switch (subjectCode) {
      case this._subjectService.biology:
      case this._subjectService.biology10:
      case this._subjectService.biology10Us:
      case this._subjectService.science:
      case this._subjectService.live:
      case this._subjectService.sciencePen:
        return Colors.primary;

      case this._subjectService.chemistry:
      case this._subjectService.chemistry10:
        return Colors.primaryChemistry;

      case this._subjectService.physics:
      case this._subjectService.physics10:
        return Colors.primaryPhysics;

      case this._subjectService.forensics:
      case this._subjectService.applied10Jpc:
        return Colors.primaryForensics;

      case this._subjectService.psychology:
        return Colors.primaryPsychology;

      case this._subjectService.marine10:
      case this._subjectService.earth10Jpc:
        return Colors.primaryMarine;

      default:
        throw new Error(`Unknown subjectCode = ${subjectCode}`);
    }
  }

  private _resolveAccentColor(subjectCode: number | undefined): string {
    assertHasValue(subjectCode);

    switch (subjectCode) {
      case this._subjectService.biology:
      case this._subjectService.biology10:
      case this._subjectService.biology10Us:
      case this._subjectService.science:
      case this._subjectService.live:
      case this._subjectService.sciencePen:
        return Colors.accent;

      case this._subjectService.chemistry:
      case this._subjectService.chemistry10:
        return Colors.accentChemistry;

      case this._subjectService.physics:
      case this._subjectService.physics10:
        return Colors.accentPhysics;

      case this._subjectService.forensics:
      case this._subjectService.applied10Jpc:
        return Colors.accentForensics;

      case this._subjectService.psychology:
        return Colors.accentPsychology;

      case this._subjectService.marine10:
      case this._subjectService.earth10Jpc:
        return Colors.accentMarine;

      default:
        throw new Error(`Unknown subjectCode = ${subjectCode}`);
    }
  }
}

export type Themes = undefined | 'chemistry-theme' | 'physics-theme' | 'forensics-theme' | 'psychology-theme' | 'marine-theme';

export interface SubjectColors {
  primary: string;
  accent: string;
}
