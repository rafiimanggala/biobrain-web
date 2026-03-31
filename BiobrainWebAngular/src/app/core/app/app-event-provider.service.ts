import { EventEmitter, Injectable } from '@angular/core';
import { Subscription } from 'rxjs';
import { SnackBarService } from '../../share/services/snack-bar.service';

@Injectable({
  providedIn: 'root',
})
export class AppEventProvider {
  private readonly _error: EventEmitter<string> = new EventEmitter<string>();

  constructor(
    private readonly _snackBarService: SnackBarService
  ) {
  }

  errorEmit(errorMessage: string): void {
    this._snackBarService.showMessage(errorMessage);
  }

  errorSubscribe(generatorOrNext?: any): Subscription {
    return this._error.subscribe(generatorOrNext);
  }
}
