import { Component, HostBinding, NgZone } from '@angular/core';

import { IHeaderAngularComp } from 'ag-grid-angular';
import { IHeaderParams } from 'ag-grid-community/dist/lib/headerRendering/header/headerComp';
import deepEqual from 'deep-equal';

export interface NavigatableHeaderParams extends IHeaderParams {
  readonly handleClick: (params: IHeaderParams) => void;
}

@Component({
  selector: 'app-ag-grid-clickable-header',
  templateUrl: './clickable-header-renderer.component.html',
  styleUrls: ['./clickable-header-renderer.component.scss'],
})
export class ClickableHeaderRendererComponent implements IHeaderAngularComp {
  params: NavigatableHeaderParams | undefined;
  @HostBinding('style.width') public width: string = '100%';

  constructor(private readonly _ngZone: NgZone) {}

  refresh(params: IHeaderParams): boolean {
    return !deepEqual(this.params, params);
  }

  agInit(params: NavigatableHeaderParams): void {
    this.params = params;
  }

  onClick(): void {
    this._ngZone.run(() => this.params?.handleClick(this.params));
  }
}
