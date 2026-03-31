import { Injectable, NgZone } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class SnackBarService {
  constructor(
    private readonly _ngZone: NgZone,
    private readonly _snackBar: MatSnackBar,
  ) {
  }

  showMessage(message: string): void {
    this._ngZone.run(() => {
      setTimeout(() => {
        const snackBar = this._snackBar.open(message, '✖', {
          duration: 5000,
          verticalPosition: 'bottom',
          panelClass: 'full-width',
        });
        snackBar.onAction().subscribe(() => {
          snackBar.dismiss();
        });
      }, 0);
    });
  }
}
