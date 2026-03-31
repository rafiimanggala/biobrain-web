import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SchoolClassChangesService {
  public readonly changes$: BehaviorSubject<string>;

  constructor() {
    this.changes$ = new BehaviorSubject('');
  }

  public next(schoolClassId: string): void {
    this.changes$.next(schoolClassId);
  }
}


