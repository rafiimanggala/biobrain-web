import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Subscription } from 'rxjs';

import { Api } from 'src/app/api/api.service';
import { AddExcludedMaterialCommand } from 'src/app/api/material-assignments/add-excluded-material.command';
import { GetExcludedMaterialsQuery } from 'src/app/api/material-assignments/get-excluded-materials.query';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { LoaderService } from 'src/app/share/services/loader.service';

import { ActiveSchoolClassService } from '../active-school-class.service';

@Injectable({
  providedIn: 'root',
})
export class ExcludedMaterialsService implements OnDestroy {
  public readonly excludedMaterialIds$ = new BehaviorSubject<string[]>([]);

  private readonly _subscriptions: Subscription[] = [];
  private _loadedForSchoolClassId: string | null = null;

  constructor(
    private readonly _api: Api,
    private readonly _activeSchoolClassService: ActiveSchoolClassService,
    private readonly _loader: LoaderService,
  ) {
    this._subscriptions.push(
      this._activeSchoolClassService.schoolClassIdChanges$.subscribe(schoolClassId => {
        if (!schoolClassId) {
          this._loadedForSchoolClassId = null;
          this.excludedMaterialIds$.next([]);
          return;
        }
        this.refresh(schoolClassId);
      }),
    );
  }

  public ngOnDestroy(): void {
    this._subscriptions.forEach(x => x.unsubscribe());
  }

  public async getExcludedMaterialIds(): Promise<string[]> {
    const schoolClassId = await this._activeSchoolClassService.schoolClassId;
    if (!schoolClassId) return [];

    if (this._loadedForSchoolClassId !== schoolClassId) {
      await this.refresh(schoolClassId);
    }
    return this.excludedMaterialIds$.value;
  }

  public async switchExcluded(materialId: string): Promise<boolean> {
    const schoolClassId = await this._activeSchoolClassService.schoolClassId;
    if (!schoolClassId) return false;

    const current = this.excludedMaterialIds$.value;
    const willExclude = !current.includes(materialId);

    try {
      this._loader.show();
      await firstValueFrom(
        this._api.send(new AddExcludedMaterialCommand(schoolClassId, materialId, willExclude)),
      );

      const next = willExclude
        ? [...current, materialId]
        : current.filter(id => id !== materialId);
      this.excludedMaterialIds$.next(next);
      return willExclude;
    } catch (e) {
      console.log(e);
      return !willExclude;
    } finally {
      this._loader.hideIfVisible();
    }
  }

  public async refresh(schoolClassId?: string | null): Promise<void> {
    const resolvedId = schoolClassId ?? await this._activeSchoolClassService.schoolClassId;
    if (!resolvedId) {
      this._loadedForSchoolClassId = null;
      this.excludedMaterialIds$.next([]);
      return;
    }

    try {
      const result = await firstValueFrom(
        this._api.send(new GetExcludedMaterialsQuery(resolvedId)),
      );
      this._loadedForSchoolClassId = resolvedId;
      this.excludedMaterialIds$.next(result?.materialIds ?? []);
    } catch (e) {
      console.log(e);
      this.excludedMaterialIds$.next([]);
    }
  }
}
