import { HttpEvent, HttpEventType, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { combineLatest, forkJoin, from, interval, Observable, of, Subscriber } from 'rxjs';
import { catchError, filter, last, map, startWith, switchMap, switchMapTo, take, tap } from 'rxjs/operators';
import { ActualContentVersion, ContentVersion } from 'src/app/api/content/content-data-models';
import { GetActualContentVersionQuery } from 'src/app/api/content/get-actual-content-version.query';
import { GetCourseContentDataQuery, GetCourseContentDataQuery_Result } from 'src/app/api/content/get-course-content-data.query';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';

import { Api } from '../../api/api.service';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { hasValue } from '../../share/helpers/has-value';
import { ObservableLock } from '../../share/helpers/obserable-lock/observable.lock';
import { ContentDownloadDialog } from '../dialogs/content-download-dialog/content-download.dialog';
import { ContentDownloadData } from '../dialogs/content-download-dialog/content-download.dialog-data';

import { ContentVersionRow, learningContentDb } from './learning-content-db';

export enum EnsureContentStates {
  unknown = 0,
  localVersionActual = 1,
  loading = 2,
  loaded = 3,
  errorWhileLoading = 4,
  localVersionExists = 5,
}

function isEnsureStateFinished(state: EnsureContentStates): boolean {
  return state === EnsureContentStates.loaded ||
    state === EnsureContentStates.localVersionActual ||
    state === EnsureContentStates.errorWhileLoading;
}

@Injectable({
  providedIn: 'root',
})
export class LearningContentLoaderService extends DisposableSubscriptionService {
  private readonly _downloadContentLock = new ObservableLock<ContentVersion[]>();

  constructor(
    private readonly _api: Api,
    private readonly _appEvents: AppEventProvider,
    private readonly _currentUserService: CurrentUserService,
    private readonly _dialog: Dialog,
  ) {
    super();

    this.subscriptions.push(
      this._currentUserService.userChanges$.pipe(
        switchMap(user => hasValue(user) ? this._getContent() : of(undefined)),
      ).subscribe(),
    );
  }

  ensureAnyVersionOfLearningContent(): Observable<EnsureContentStates> {
    return new Observable<EnsureContentStates>(subscriber => {
      const ensureSubscription = from(learningContentDb.getContentVersion()).pipe(
        tap(localVer => {
          if (localVer && localVer.every(x => hasValue(x.courseId) && x.courseId != '')) {
            subscriber.next(EnsureContentStates.localVersionExists);
            subscriber.complete();
          }
        }),
        switchMap(() => this._currentUserService.user),
        tap((user) => {
          if (!hasValue(user)) {
            subscriber.next(EnsureContentStates.errorWhileLoading);
            subscriber.complete();
            return;
          }
          if (!user?.isStudent() && !user?.isTeacher()) {
            subscriber.next(EnsureContentStates.localVersionExists);
            subscriber.complete();
            return;
          }
        }),
        switchMap((user) => this._api.send(new GetActualContentVersionQuery(user?.userId ?? ''))),
        switchMap(actualVer => this._initiateLoading(actualVer, subscriber))
      ).subscribe();

      return () => {
        ensureSubscription.unsubscribe();
      };
    });
  }

  ensureActualLearningContent(): Observable<EnsureContentStates> {
    return new Observable<EnsureContentStates>(subscriber => {
      const ensureSubscription = forkJoin([
        from(this._currentUserService.user).pipe(
          tap((user) => {
            if (!hasValue(user)) {
              subscriber.next(EnsureContentStates.errorWhileLoading);
              subscriber.complete();
              return;
            }
            if (!user?.isStudent() && !user?.isTeacher()) {
              subscriber.next(EnsureContentStates.localVersionActual);
              subscriber.complete();
              return;
            }
          }),
          switchMap((user) => this._api.send(new GetActualContentVersionQuery(user?.userId ?? '')))
        ),
        learningContentDb.getContentVersion(),
      ]).pipe(
        tap(([actualVer, localVer]) => {
          if (isLocalVersionActual(actualVer, localVer)) {
            subscriber.next(EnsureContentStates.localVersionActual);
            subscriber.complete();
          }
        }),
        filter(([actualVer, localVer]) => !isLocalVersionActual(actualVer, localVer)),
        map(([actualVer, _]) => actualVer),
        switchMap(actualVer => this._initiateLoading(actualVer, subscriber))
      ).subscribe();

      return () => {
        ensureSubscription.unsubscribe();
      };
    });
  }

  private _initiateLoading(actualVer: ActualContentVersion[], subscriber: Subscriber<EnsureContentStates>): Observable<ContentVersion[]> {
    subscriber.next(EnsureContentStates.loading);

    return this._downloadContentLock.lock(this._downloadContent(actualVer)).pipe(
      tap({
        next: undefined,
        error: () => {
          subscriber.next(EnsureContentStates.errorWhileLoading);
          subscriber.complete();
        },
        complete: () => {
          subscriber.next(EnsureContentStates.loaded);
          subscriber.complete();
        }
      })
    );
  }

  private _getContent(): Observable<EnsureContentStates | undefined> {
    const periodMs = 10 * 60 * 1000;

    return interval(periodMs).pipe(
      startWith(periodMs),
      switchMap(() => this.ensureActualLearningContent().pipe(
        filter(x => isEnsureStateFinished(x)),
        take(1))
      ),
      catchError(err => {
        console.error(err);

        if ('message' in err) {
          this._appEvents.errorEmit('Unable to lock content');
          // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
          this._appEvents.errorEmit(err.message);
        } else {
          this._appEvents.errorEmit(err);
        }

        return of(undefined);
      }),
    );
  }

  private _downloadContent(actualVer: ActualContentVersion[]): Observable<ContentVersion[]> {
    return from(learningContentDb.getContentVersion()).pipe(
      filter(localVer => !isLocalVersionActual(actualVer, localVer)),
      map((localVer) => this._getContentData(localVer, actualVer)),  
      tap(content => this._openDialogIfNeed(content)),    
      switchMap(content => learningContentDb.saveContent(content, actualVer))
    );
  }

  private _getContentData(local: ContentVersionRow[], actual: ActualContentVersion[]): ContentDownloadData[] {
    var toDownload = actual.filter(x => {
      let localVersion = local.find(_ => _.courseId == x.courseId);
      if (!hasValue(localVersion)) return true;
      return localVersion.version < x.version;
    });

    var downloads = toDownload.map((course) => {
      return new ContentDownloadData(course.courseId, course.courseName, this._downloadCourse(course.courseId), local.find(_ => _.courseId == course.courseId));
    });

    return downloads;
  }

  private _openDialogIfNeed(content: ContentDownloadData[]): ContentDownloadData[] {    
    // Hack to show dialog only when no any version
    var dialogData = content.filter(_ => {
      var version = _.localVersion;
      if(!version) return true;
      return !version.version || version.version < 1;
    });
    if(dialogData.length > 0)
      this._dialog.show(ContentDownloadDialog, dialogData, {}, () => location.reload());
    return content;
  }

  private _downloadCourse(courseId: string): Observable<HttpEvent<GetCourseContentDataQuery_Result>> {
    return this._api.observe(new GetCourseContentDataQuery(courseId));
  }
}

function isLocalVersionActual(
  actualVersion: ContentVersion[],
  localVersion: ContentVersion[] | undefined | null,
): boolean {
  if (!localVersion) {
    return false;
  }

  return actualVersion.every(x => {
    let local = localVersion.find(_ => _.courseId == x.courseId);
    if (!hasValue(local)) return false;
    return local.version >= x.version;
  });
}
