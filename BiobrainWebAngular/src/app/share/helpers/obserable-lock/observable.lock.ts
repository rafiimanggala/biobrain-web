import { EMPTY, Observable } from 'rxjs';
import { share, tap } from 'rxjs/operators';

import { Logger } from './logger';
import { NoneLogger } from './none.logger';

export class ObservableLock<T> {
  private _isLocked = false;
  private _lockedStreamWrapper: Observable<T>;
  private readonly _logger: Logger;

  constructor(logger?: Logger) {
    this._logger = logger ?? new NoneLogger();
    this._lockedStreamWrapper = EMPTY;
  }

  public lock(stream$: Observable<T>): Observable<T> {
    if (this._isLocked) {
      this._logger.log('lock', '[locked] -> wait...');
      return this._lockedStreamWrapper.pipe(
        tap(
          {
            next: undefined,
            error: e => this._logger.error('lock', '[locked] -> stream failed.', e),
            complete: () => this._logger.log('lock', '[locked] -> stream completed.')
          }
        )
      );
    }

    this._logger.log('lock', '[acquiring lock]');
    this._isLocked = true;
    this._lockedStreamWrapper = stream$.pipe(share());
    this._logger.log('lock', '[lock acquired]');

    return this._lockedStreamWrapper.pipe(
      tap({
        next: undefined,
        error: e => {
          this._logger.error('lock', '[stream failed] -> releasing lock.', e);
          this._isLocked = false;
          this._logger.error('lock', '[stream failed] -> lock released.', e);
        },
        complete: () => {
          this._logger.log('lock', '[stream completed] -> releasing lock.');
          this._isLocked = false;
          this._logger.log('lock', '[stream completed] -> lock released.');
        }
      })
    );
  }
}
