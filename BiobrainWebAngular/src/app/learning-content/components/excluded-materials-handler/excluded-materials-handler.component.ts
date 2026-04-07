import { Component, EventEmitter, OnDestroy, Output } from '@angular/core';
import { Subscription } from 'rxjs';

import { ExcludedMaterialsService } from 'src/app/core/services/excluded-materials/excluded-materials.service';
import { RequestHandlingService } from 'src/app/share/services/request-parse.service';

@Component({
  selector: 'app-excluded-materials-handler',
  template: '',
})
export class ExcludedMaterialsHandlerComponent implements OnDestroy {
  @Output() public excludedChanged = new EventEmitter<void>();

  private readonly _subscriptions: Subscription[] = [];

  constructor(
    private readonly _requestService: RequestHandlingService,
    private readonly _excludedMaterialsService: ExcludedMaterialsService,
  ) {
    this._subscriptions.push(
      this._requestService.switchExcludedMaterial$.subscribe({
        next: event => this._handle(event),
      }),
    );
  }

  public ngOnDestroy(): void {
    this._subscriptions.forEach(x => x.unsubscribe());
  }

  private async _handle(event: { materialId: string }): Promise<void> {
    if (!event.materialId) return;
    await this._excludedMaterialsService.switchExcluded(event.materialId);
    this.excludedChanged.emit();
  }
}
